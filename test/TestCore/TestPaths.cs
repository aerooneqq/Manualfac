using NUnit.Framework;

namespace TestCore;

public static class TestPaths
{
  public static string GetPathToTestConsoleAppSolutionFolder()
  {
    var path = Path.Combine(GetTestDataPath(), "solutions", "TestConsoleApp");
    Assert.That(Directory.Exists(path), Is.True);

    return path;
  }

  private static string GetTestDataPath()
  {
    var dir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.FullName!;
    var testDataPath = Path.Combine(dir, "test_data");
    Assert.That(Directory.Exists(testDataPath), Is.True);

    return testDataPath;
  }

  public static string GetPathToSources(string sourcesName)
  {
    var path = Path.Combine(GetTestDataPath(), "sources", sourcesName);
    Assert.That(Directory.Exists(path), Is.True);

    return path;
  }

  public static string GetPathToGoldDir()
  {
    var path = Path.Combine(GetTestDataPath(), "gold");
    Assert.That(Directory.Exists(path), Is.True);

    return path;
  }

  public static string GetPathToGoldDirFor(string sourceName)
  {
    var path = Path.Combine(GetTestDataPath(), "gold", sourceName);

    if (!Directory.Exists(path))
    {
      Directory.CreateDirectory(path);
    }

    Assert.That(Directory.Exists(path), Is.True);

    return path;
  }

  public static string GetPathToIntegrationTestFolder(string solutionName, string projectName)
  {
    var path = Path.Combine(GetPathToGoldDir(), solutionName, projectName);
    if (!Directory.Exists(path))
    {
      Directory.CreateDirectory(path);
    }

    return path;
  }
}