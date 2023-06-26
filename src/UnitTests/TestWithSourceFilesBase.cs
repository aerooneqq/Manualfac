using ManualfacAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TestCore;

namespace UnitTests;

public abstract class TestWithSourceFilesBase
{
  private readonly record struct GeneratedFile(string Name, string Text);
  
  protected void DoTest(Func<Compilation, IReadOnlyList<SyntaxTree>> actualTest)
  {
    var testName = TestContext.CurrentContext.Test.Name;
    var pathToSources = TestPaths.GetPathToSources(testName);
    var files = Directory.EnumerateFiles(pathToSources).Where(path => path.EndsWith(".cs"));
    var generatedTrees = actualTest(CreateCompilation(files));
    var generatedFiles = generatedTrees
      .Select(tree => new GeneratedFile(tree.FilePath[(tree.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..], tree.GetText().ToString()))
      .ToDictionary(static file => file.Name, static file => file.Text);
    
    var goldFolderPath = TestPaths.GetPathToGoldDirFor(testName);
    var goldFileNames = Directory.EnumerateFiles(goldFolderPath).Select(Path.GetFileName).ToHashSet();
    var differences = new HashSet<string>();

    foreach (var generatedFileName in generatedFiles.Keys)
    {
      if (!goldFileNames.Contains(generatedFileName))
      {
        differences.Add(generatedFileName);
      }
    }
    
    FailWithDifferencesIfNeeded($"Generated files: {string.Join(',', differences)} were not in gold");
    
    foreach (var goldFileName in goldFileNames)
    {
      if (!generatedFiles.ContainsKey(goldFileName))
      {
        Assert.Fail($"Gold file {goldFileName} was not generated");
      }
    }

    foreach (var (generatedFileName, generatedText) in generatedFiles)
    {
      var goldText = File.ReadAllText(Path.Join(goldFolderPath, generatedFileName));
      if (goldText != generatedText)
      {
        differences.Add(generatedFileName);
      }
    }
    
    FailWithDifferencesIfNeeded($"Following files have different generated content: {string.Join(',', differences)}");
    return;

    void FailWithDifferencesIfNeeded(string message)
    {
      if (differences.Count != 0)
      {
        foreach (var difference in differences)
        {
          WriteTmpFile(goldFolderPath, difference, generatedFiles[difference]);
        }

        Assert.Fail(message);
      }
      
      differences.Clear();
    }
  }

  private static void WriteTmpFile(string goldDirectory, string originalFileName, string text)
  {
    File.WriteAllText(Path.Combine(goldDirectory, $"{originalFileName}.tmp"), text);
  }
  
  private static Compilation CreateCompilation(IEnumerable<string> files)
  {
    return CSharpCompilation.Create(
      "compilation", 
      files.Select(File.ReadAllText).Select(text => CSharpSyntaxTree.ParseText(text)).ToArray(),
      CreateMetadataReferences(),
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
  }
  
  private static IEnumerable<MetadataReference> CreateMetadataReferences() =>
    AppDomain.CurrentDomain.GetAssemblies()
      .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
      .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
      .Concat(new[]
      {
        MetadataReference.CreateFromFile(typeof(ManualfacAttribute).Assembly.Location)
      });
}