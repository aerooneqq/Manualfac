using Manualfac.Generators;
using ManualfacAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UnitTests;

[TestFixture]
public class GeneratedFilesTest 
{
  [Test]
  public void DoTest()
    {
        var code = @"
using ManualfacAttributes;

namespace asd;

[Component]
public partial class Class1
{
}
";
    
    GeneratorDriver driver = CSharpGeneratorDriver.Create(new ServiceInjectionGenerator());

    driver.RunGeneratorsAndUpdateCompilation(CreateCompilation(code), out var outputCompilation, out var diagnostics);
    Assert.Multiple(() =>
    {
      Assert.That(diagnostics.IsEmpty, Is.True);
      Assert.That(outputCompilation.SyntaxTrees.Count(), Is.EqualTo(3));
    });
  }

  private static Compilation CreateCompilation(string source)
  {
    return CSharpCompilation.Create(
      "compilation", 
      new[] { CSharpSyntaxTree.ParseText(source) },
      CreateMetadataReferences(),
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
  }

  private static IEnumerable<MetadataReference> CreateMetadataReferences() =>
    AppDomain.CurrentDomain.GetAssemblies()
      .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
      .Select(name => MetadataReference.CreateFromFile(name.Location))
      .Concat(new[]
      {
        MetadataReference.CreateFromFile(typeof(ManualfacAttribute).Assembly.Location)
      });
}