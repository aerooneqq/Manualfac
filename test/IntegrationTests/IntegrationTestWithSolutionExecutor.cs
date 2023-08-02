using System.Diagnostics;
using System.Text;
using TestCore;

namespace GeneratorsTests;

public class IntegrationTestWithSolutionExecutor
{
  private static readonly int ourTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

  private readonly string mySolutionDirectory;


  public IntegrationTestWithSolutionExecutor(string solutionDirectory)
  {
    mySolutionDirectory = solutionDirectory;
  }


  public void Execute()
  {
    DeleteGeneratedFolder();
    DotnetClean();
    DotnetRebuild();
    CompareGoldAndTestValue();
  }

  private void CompareGoldAndTestValue()
  {
    var solutionName = Path.GetFileNameWithoutExtension(mySolutionDirectory);
    foreach (var (projectName, genSourcesFolderPath) in FindObjPaths())
    {
      Assert.That(Directory.Exists(genSourcesFolderPath));
      var generatedFiles = Directory.EnumerateFiles(genSourcesFolderPath, "*.cs", SearchOption.TopDirectoryOnly);
      var goldFolder = TestPaths.GetPathToIntegrationTestFolder(solutionName, projectName);
      var contents = generatedFiles.ToDictionary(file => Path.GetFileName(file), File.ReadAllText);
      new GoldComparisonExecutor(goldFolder, contents).Execute();
    }
  }

  private IEnumerable<(string ProjectName, string GenSourcesFolderPath)> FindObjPaths() =>
    Directory.EnumerateFiles(mySolutionDirectory, "*.csproj", SearchOption.AllDirectories)
      .Select(path =>
      {
        var projectName = Path.GetFileNameWithoutExtension(path);
        var dirName = Path.GetDirectoryName(path);
        Assert.That(dirName, Is.Not.Null);
        return (ProjectName: projectName, ObjFolderPath: dirName!);
      })
      .Select(tuple => (tuple.ProjectName, CreatePathToGenDirectory(tuple.ObjFolderPath)));

  private static string CreatePathToGenDirectory(string objFolderPath)
  {
    const string Manualfac = "Manualfac";
    const string GeneratorName = "ManualfacGenerator";
    const string GeneratorFolderName = $"Manualfac.Generators.{GeneratorName}";

    return Path.Combine(objFolderPath, "obj", "Debug", "net7.0", "generated", Manualfac, GeneratorFolderName);
  }

  private void DeleteGeneratedFolder()
  {
    foreach (var (_, genSourcesFolderPath) in FindObjPaths())
    {
      try
      {
        if (Directory.Exists(genSourcesFolderPath))
        {
          Directory.Delete(genSourcesFolderPath, true);
        }
      }
      catch (Exception ex)
      {
        Assert.Fail($"Failed to delete folder {genSourcesFolderPath}, exception: {ex}");
      }
    }
  }

  private void DotnetClean() => LaunchDotnetProcess(mySolutionDirectory, "clean");
  private void DotnetRebuild() => LaunchDotnetProcess(mySolutionDirectory, "build --no-incremental");

  private static void LaunchDotnetProcess(string workingDirectory, string args)
  {
    const string Dotnet = "dotnet";

    var process = new Process
    {
      StartInfo = new ProcessStartInfo
      {
        WorkingDirectory = workingDirectory,
        FileName = Dotnet,
        Arguments = args,
        CreateNoWindow = true,
        UseShellExecute = false,
        RedirectStandardOutput = true
      }
    };

    process.Start();

    var output = new StringBuilder();
    process.OutputDataReceived += (_, eventArgs) => { output.Append(eventArgs.Data); };

    process.BeginOutputReadLine();

    if (!process.WaitForExit(ourTimeout))
    {
      process.Kill();
      Assert.Fail($"Process {CreateProcessName()} timeoutted");
      return;
    }

    if (process.ExitCode != 0)
    {
      Assert.Fail($"Process {CreateProcessName()} exited with exit code {process.ExitCode}, output: {output}");
    }

    string CreateProcessName() => $"{Dotnet} {args}";
  }
}