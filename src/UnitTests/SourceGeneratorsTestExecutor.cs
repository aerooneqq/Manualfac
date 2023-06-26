﻿using System.Diagnostics;
using ManualfacAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TestCore;

namespace UnitTests;

internal class SourceGeneratorsTestExecutor
{
  private readonly record struct GeneratedFile(string Name, string Text);

  
  private readonly string myTestName;
  private readonly Func<Compilation, IReadOnlyList<SyntaxTree>> myActualTest;
  private readonly HashSet<string> myDifferences;
  private readonly string myPathToGoldDirFor;

  private Dictionary<string, string>? myGeneratedFiles;
  private HashSet<string>? myGoldFileNames;


  public SourceGeneratorsTestExecutor(string testName, Func<Compilation, IReadOnlyList<SyntaxTree>> actualTest)
  {
    myDifferences = new HashSet<string>();
    myTestName = testName;
    myActualTest = actualTest;
    myPathToGoldDirFor = TestPaths.GetPathToGoldDirFor(myTestName);
  }

  
  public void ExecuteTest()
  {
    DoExecuteTest();
    AssertAllGeneratedFilesInGold();
    AssertAllGoldInGeneratedFiles();
    AssertGeneratedFilesAndGoldSameContent();
  }

  private void DoExecuteTest()
  {
    var pathToSources = TestPaths.GetPathToSources(myTestName);
    var files = Directory.EnumerateFiles(pathToSources).Where(path => path.EndsWith(".cs"));
    
    var generatedTrees = myActualTest(CreateCompilation(files));
    
    myGeneratedFiles = generatedTrees
      .Select(tree => new GeneratedFile(tree.FilePath[(tree.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..], tree.GetText().ToString().Replace("\r\n", "\n")))
      .ToDictionary(static file => file.Name, static file => file.Text);
    
    myGoldFileNames = Directory.EnumerateFiles(myPathToGoldDirFor)
      .Select(Path.GetFileName)
      .Where(name => name.EndsWith(".cs"))
      .ToHashSet();
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
      var goldText = File.ReadAllText(Path.Join(myPathToGoldDirFor, generatedFileName));
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
        WriteTmpFile(myPathToGoldDirFor, difference, myGeneratedFiles[difference]);
      }

      Assert.Fail(message);
    }
      
    myDifferences.Clear();
  }
  
  private static void WriteTmpFile(string goldDirectory, string originalFileName, string text) => 
    File.WriteAllText(Path.Combine(goldDirectory, $"{originalFileName}.tmp"), text);

  private static Compilation CreateCompilation(IEnumerable<string> files) =>
    CSharpCompilation.Create(
      "compilation", 
      files.Select(File.ReadAllText).Select(text => CSharpSyntaxTree.ParseText(text)).ToArray(),
      CreateMetadataReferences(),
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

  private static IEnumerable<MetadataReference> CreateMetadataReferences() =>
    AppDomain.CurrentDomain.GetAssemblies()
      .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
      .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
      .Concat(new[]
      {
        MetadataReference.CreateFromFile(typeof(ManualfacAttribute).Assembly.Location)
      });
}