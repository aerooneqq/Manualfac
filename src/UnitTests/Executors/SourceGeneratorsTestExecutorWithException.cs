using Microsoft.CodeAnalysis;

namespace UnitTests.Executors;

internal class SourceGeneratorsTestExecutorWithException<TGenerator, TException> : SourceGeneratorTestExecutorBase<TGenerator>
  where TGenerator : ISourceGenerator, new()
  where TException : Exception
{
  public SourceGeneratorsTestExecutorWithException(string testName) : base(testName)
  {
  }
  
  
  public override void ExecuteTest()
  {
    base.ExecuteTest();
    
    Assert.That(RunResult, Is.Not.Null);
    Assert.That(RunResult!.Results.First().Exception is TException);
  }
}