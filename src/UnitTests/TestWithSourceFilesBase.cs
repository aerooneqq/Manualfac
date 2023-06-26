using Microsoft.CodeAnalysis;

namespace UnitTests;

public abstract class TestWithSourceFilesBase
{
  protected static void DoTest(Func<Compilation, IReadOnlyList<SyntaxTree>> actualTest) => 
    new SourceGeneratorsTestExecutor(TestContext.CurrentContext.Test.Name, actualTest).ExecuteTest();
}