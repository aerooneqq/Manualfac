using Manualfac.Generators;
using Microsoft.CodeAnalysis.CSharp;

namespace UnitTests;

[TestFixture]
public class GeneratedFilesTest : TestWithSourceFilesBase
{
  [Test]
  public void SimpleTest() => DoTest(compilation =>
  {
    var driver = CSharpGeneratorDriver.Create(new ServiceInjectionGenerator());
    driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
    
    Assert.Multiple(() =>
    {
      Assert.That(diagnostics.IsEmpty, Is.True);
      Assert.That(outputCompilation.SyntaxTrees.Count(), Is.EqualTo(3));
    });
  });
}