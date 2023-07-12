using TestCore;

namespace GeneratorsTests;

[TestFixture]
public class Tests
{
  [Test]
  public void TestTestConsoleApp() =>
    new IntegrationTestWithSolutionExecutor(TestPaths.GetPathToTestConsoleAppSolutionFolder()).Execute();
}