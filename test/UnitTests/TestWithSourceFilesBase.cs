using Microsoft.CodeAnalysis;
using UnitTests.Executors;

namespace UnitTests;

public abstract class TestWithSourceFilesBase<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
  private static string TestName => TestContext.CurrentContext.Test.Name;


  protected static void DoTest() =>
    new SourceGeneratorsTestExecutor<TGenerator>(TestName).ExecuteTest();

  protected static void DoTestWithException<TException>() where TException : Exception =>
    new SourceGeneratorsTestExecutorWithException<TGenerator, TException>(TestName).ExecuteTest();

  protected static void DoTestWithDiagnostic(string errorId) =>
    new SourceGeneratorsTestExecutorWithDiagnostic<TGenerator>(TestName, errorId).ExecuteTest();
}