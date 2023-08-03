using Microsoft.CodeAnalysis;

namespace UnitTests.Executors;

internal class SourceGeneratorsTestExecutorWithException<TGenerator, TException>(string testName) : SourceGeneratorTestExecutorBase<TGenerator>(testName)
  where TGenerator : IIncrementalGenerator, new()
  where TException : Exception
{
  public override void ExecuteTest()
  {
    base.ExecuteTest();

    Assert.That(RunResult, Is.Not.Null);
    Assert.That(RunResult!.Results.First().Exception is TException);
  }
}