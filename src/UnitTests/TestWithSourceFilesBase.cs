using ManualfacAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TestCore;

namespace UnitTests;

public abstract class TestWithSourceFilesBase
{
  protected void DoTest(Action<Compilation> actualTest)
  {
    var testName = TestContext.CurrentContext.Test.Name;
    var pathToSources = TestPaths.GetPathToSources(testName);
    var files = Directory.EnumerateFiles(pathToSources).Where(path => path.EndsWith(".cs"));
    actualTest(CreateCompilation(files));
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