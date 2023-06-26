using Microsoft.CodeAnalysis;

namespace UnitTests;

public abstract class TestWithSourceFilesBase<TGenerator> where TGenerator : ISourceGenerator, new()
{
  protected static void DoTest() => 
    new SourceGeneratorsTestExecutor<TGenerator>(TestContext.CurrentContext.Test.Name).ExecuteTest();
}