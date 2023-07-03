using System.Diagnostics;
using TestCore;

namespace GeneratorsTests;

[TestFixture]
public class Tests
{
  [Test]
  public void TestTestConsoleApp()
  {
    var path = TestPaths.GetPathToTestConsoleApp();
    var process = new Process
    {
      StartInfo = new ProcessStartInfo
      {
        FileName = "dotnet",
        Arguments = $"run --project {path}",
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        UseShellExecute = false
      }
    };

    process.Start();
    if (!process.WaitForExit((int)TimeSpan.FromSeconds(5).TotalMilliseconds))
    {
      Assert.Fail("Test process timeout");
    }

    var output = process.StandardOutput.ReadToEnd();
    var expected = string.Join(string.Empty, Enumerable.Range(0, 16).Select(_ => "True "));
    Assert.That(output.Replace("\r\n", " ").Replace("\n", " "), Is.EqualTo(expected));
  }
}