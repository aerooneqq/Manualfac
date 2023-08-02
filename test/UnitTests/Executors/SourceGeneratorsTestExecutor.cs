using Microsoft.CodeAnalysis;
using TestCore;

namespace UnitTests.Executors;

internal class SourceGeneratorsTestExecutor<TGenerator> : SourceGeneratorTestExecutorBase<TGenerator>
  where TGenerator : IIncrementalGenerator, new()
{
  private readonly string myPathToGoldDirFor;


  public SourceGeneratorsTestExecutor(string testName) : base(testName)
  {
    myPathToGoldDirFor = TestPaths.GetPathToGoldDirFor(testName);
  }


  public override void ExecuteTest()
  {
    base.ExecuteTest();

    Assert.Multiple(() =>
    {
      Assert.That(RunResult, Is.Not.Null);
      Assert.That(RunResult!.Diagnostics.IsEmpty);
      Assert.That(GeneratedFiles is { }, Is.True);
    });

    new GoldComparisonExecutor(myPathToGoldDirFor, GeneratedFiles!).Execute();
  }
}