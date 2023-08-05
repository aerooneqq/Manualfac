using Microsoft.CodeAnalysis;

namespace UnitTests.Executors;

internal class SourceGeneratorsTestExecutorWithDiagnostic<TGenerator>(string testName, string errorId)
  : SourceGeneratorTestExecutorBase<TGenerator>(testName)
  where TGenerator : IIncrementalGenerator, new()
{
  public override void ExecuteTest()
  {
    base.ExecuteTest();

    Assert.That(RunResult, Is.Not.Null);

    var diagnostic = RunResult!.Results.First().Diagnostics;

    Assert.That(diagnostic, Has.Length.EqualTo(1));
    Assert.That(diagnostic[0].Id, Is.EqualTo(errorId));
  }
}