using System.Diagnostics;
using NUnit.Framework;

namespace TestCore;

public class GoldComparisonExecutor
{
  private readonly string myGoldDirectory;
  private readonly Dictionary<string, string> myGeneratedFiles;
  private readonly HashSet<string> myGoldFileNames;
  private readonly HashSet<string> myDifferences;


  public GoldComparisonExecutor(string goldDirectory, Dictionary<string, string> generatedFiles)
  {
    myDifferences = new HashSet<string>();
    myGoldDirectory = goldDirectory;
    myGeneratedFiles = generatedFiles;

    myGoldFileNames = Directory.EnumerateFiles(goldDirectory)
      .Select(Path.GetFileName)
      .Where(name => name.EndsWith(".cs"))
      .ToHashSet();
  }


  public void Execute()
  {
    AssertAllGeneratedFilesInGold();
    AssertAllGoldInGeneratedFiles();
    AssertGeneratedFilesAndGoldSameContent();
  }

  private void AssertAllGeneratedFilesInGold()
  {
    Debug.Assert(myGeneratedFiles is { });
    Debug.Assert(myGoldFileNames is { });

    foreach (var generatedFileName in myGeneratedFiles.Keys)
    {
      if (!myGoldFileNames.Contains(generatedFileName))
      {
        myDifferences.Add(generatedFileName);
      }
    }

    FailWithDifferencesIfNeeded($"Generated files: {string.Join(',', myDifferences)} were not in gold");
  }

  private void AssertAllGoldInGeneratedFiles()
  {
    Debug.Assert(myGeneratedFiles is { });
    Debug.Assert(myGoldFileNames is { });

    foreach (var goldFileName in myGoldFileNames)
    {
      if (!myGeneratedFiles.ContainsKey(goldFileName))
      {
        Assert.Fail($"Gold file {goldFileName} was not generated");
      }
    }
  }

  private void AssertGeneratedFilesAndGoldSameContent()
  {
    Debug.Assert(myGeneratedFiles is { });
    Debug.Assert(myGoldFileNames is { });

    foreach (var (generatedFileName, generatedText) in myGeneratedFiles)
    {
      var goldText = File.ReadAllText(Path.Join(myGoldDirectory, generatedFileName)).ReplaceRn();
      if (goldText != generatedText)
      {
        myDifferences.Add(generatedFileName);
      }
    }

    FailWithDifferencesIfNeeded($"Following files have different generated content: {string.Join(',', myDifferences)}");
  }

  private void FailWithDifferencesIfNeeded(string message)
  {
    Debug.Assert(myGeneratedFiles is { });
    if (myDifferences.Count != 0)
    {
      foreach (var difference in myDifferences)
      {
        WriteTmpFile(myGoldDirectory, difference, myGeneratedFiles[difference]);
      }

      Assert.Fail(message);
    }

    myDifferences.Clear();
  }

  private static void WriteTmpFile(string goldDirectory, string originalFileName, string text) =>
    File.WriteAllText(Path.Combine(goldDirectory, $"{originalFileName}.tmp"), text.ReplaceRn());
}