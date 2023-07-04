using System.Diagnostics;
using Microsoft.CodeAnalysis;
using TestCore;

namespace UnitTests.Executors;


internal class SourceGeneratorsTestExecutor<TGenerator> : SourceGeneratorTestExecutorBase<TGenerator> 
  where TGenerator : ISourceGenerator, new()
{
  private readonly HashSet<string> myDifferences;
  private readonly string myPathToGoldDirFor;
  
  private HashSet<string>? myGoldFileNames;


  public SourceGeneratorsTestExecutor(string testName) : base(testName)
  {
    myDifferences = new HashSet<string>();
    myPathToGoldDirFor = TestPaths.GetPathToGoldDirFor(testName);
  }


  public override void ExecuteTest()
  {
    base.ExecuteTest();
    
    Assert.Multiple(() =>
    {
      Assert.That(RunResult, Is.Not.Null);
      Assert.That(RunResult!.Diagnostics.IsEmpty);
    });
    
    myGoldFileNames = Directory.EnumerateFiles(myPathToGoldDirFor)
      .Select(Path.GetFileName)
      .Where(name => name.EndsWith(".cs"))
      .ToHashSet();
  
    AssertAllGeneratedFilesInGold();
    AssertAllGoldInGeneratedFiles();
    AssertGeneratedFilesAndGoldSameContent();
  }

  private void AssertAllGeneratedFilesInGold()
  {
    Debug.Assert(GeneratedFiles is { });
    Debug.Assert(myGoldFileNames is { });
    
    foreach (var generatedFileName in GeneratedFiles.Keys)
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
    Debug.Assert(GeneratedFiles is { });
    Debug.Assert(myGoldFileNames is { });
    
    foreach (var goldFileName in myGoldFileNames)
    {
      if (!GeneratedFiles.ContainsKey(goldFileName))
      {
        Assert.Fail($"Gold file {goldFileName} was not generated");
      }
    }
  }

  private void AssertGeneratedFilesAndGoldSameContent()
  {
    Debug.Assert(GeneratedFiles is { });
    Debug.Assert(myGoldFileNames is { });
    
    foreach (var (generatedFileName, generatedText) in GeneratedFiles)
    {
      var goldText = File.ReadAllText(Path.Join(myPathToGoldDirFor, generatedFileName)).ReplaceRn();
      if (goldText != generatedText)
      {
        myDifferences.Add(generatedFileName);
      }
    }
    
    FailWithDifferencesIfNeeded($"Following files have different generated content: {string.Join(',', myDifferences)}");
  }
  
  private void FailWithDifferencesIfNeeded(string message)
  {
    Debug.Assert(GeneratedFiles is { });
    if (myDifferences.Count != 0)
    {
      foreach (var difference in myDifferences)
      {
        WriteTmpFile(myPathToGoldDirFor, difference, GeneratedFiles[difference]);
      }

      Assert.Fail(message);
    }
      
    myDifferences.Clear();
  }
  
  private static void WriteTmpFile(string goldDirectory, string originalFileName, string text) => 
    File.WriteAllText(Path.Combine(goldDirectory, $"{originalFileName}.tmp"), text.ReplaceRn());
}