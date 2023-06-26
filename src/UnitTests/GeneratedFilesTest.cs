using Manualfac.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UnitTests;

[TestFixture]
public class GeneratedFilesTest : TestWithSourceFilesBase
{
  [Test]
  public void SimpleTest() => DoTest(compilation =>
  {
    GeneratorDriver driver = CSharpGeneratorDriver.Create(new ServiceInjectionGenerator());
    driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
    
    Assert.Multiple(() =>
    {
      Assert.That(diagnostics.IsEmpty, Is.True);
      Assert.That(outputCompilation.SyntaxTrees.Count(), Is.EqualTo(3));
    });

    return driver.GetRunResult().GeneratedTrees;
  });
}