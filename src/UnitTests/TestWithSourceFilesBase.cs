using Microsoft.CodeAnalysis;
using UnitTests.Executors;

namespace UnitTests;

public abstract class TestWithSourceFilesBase<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
  protected static void DoTest() => 
    new SourceGeneratorsTestExecutor<TGenerator>(TestContext.CurrentContext.Test.Name).ExecuteTest();

  protected static void DoTestWithException<TException>() where TException : Exception =>
    new SourceGeneratorsTestExecutorWithException<TGenerator, TException>(TestContext.CurrentContext.Test.Name).ExecuteTest();
}