using NUnit.Framework;

namespace TestCore;

public static class TestPaths
{
  public static string GetPathToTestConsoleApp()
  {
    var path = Path.Combine(GetTestDataPath(), "solutions", "TestConsoleApp", "TestConsoleApp", "TestConsoleApp.csproj");
    Assert.That(File.Exists(path), Is.True);

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
}