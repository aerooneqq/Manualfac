using ManualfacAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TestCore;

namespace UnitTests.Executors;

internal abstract class SourceGeneratorTestExecutorBase<TGenerator> 
  where TGenerator : IIncrementalGenerator, new()
{
  private readonly string myTestName;

  protected Dictionary<string, string>? GeneratedFiles;
  protected GeneratorDriverRunResult? RunResult;


  protected SourceGeneratorTestExecutorBase(string testName)
  {
    myTestName = testName;
  }

  
  public virtual void ExecuteTest()
  {
    var pathToSources = TestPaths.GetPathToSources(myTestName);
    var files = Directory.EnumerateFiles(pathToSources).Where(path => path.EndsWith(".cs"));
    
    GeneratorDriver driver = CSharpGeneratorDriver.Create(new TGenerator());
    driver = driver.RunGeneratorsAndUpdateCompilation(CreateCompilation(files), out _, out _);
    
    RunResult = driver.GetRunResult();
    
    GeneratedFiles = RunResult.GeneratedTrees
      .Select(tree => tree.ToGeneratedFile())
      .ToDictionary(static file => file.Name, static file => file.Text);
  }
  
  private static Compilation CreateCompilation(IEnumerable<string> files) =>
    CSharpCompilation.Create(
      "compilation", 
      files.Select(File.ReadAllText).Select(text => CSharpSyntaxTree.ParseText(text)).ToArray(),
      CreateMetadataReferences(),
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

  private static IEnumerable<MetadataReference> CreateMetadataReferences() =>
    AppDomain.CurrentDomain.GetAssemblies()
      .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
      .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
      .Concat(new[]
      {
        MetadataReference.CreateFromFile(typeof(ManualfacAttribute).Assembly.Location)
      });
}