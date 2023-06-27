using System.Diagnostics;
using ManualfacAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TestCore;

namespace UnitTests;

internal readonly record struct GeneratedFile(string Name, string Text);

internal static class SyntaxTreeExtensions
{
  public static GeneratedFile ToGeneratedFile(this SyntaxTree tree) =>
    new(
      tree.FilePath[(tree.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..],
      tree.GetText().ToString().Replace("\r\n", "\n")
    );
}

internal class SourceGeneratorsTestExecutor<TGenerator> where TGenerator : ISourceGenerator, new()
{
  private readonly string myTestName;
  private readonly HashSet<string> myDifferences;
  private readonly string myPathToGoldDirFor;

  private Dictionary<string, string>? myGeneratedFiles;
  private HashSet<string>? myGoldFileNames;


  public SourceGeneratorsTestExecutor(string testName)
  {
    myDifferences = new HashSet<string>();
    myTestName = testName;
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
    
    GeneratorDriver driver = CSharpGeneratorDriver.Create(new TGenerator());
    driver = driver.RunGeneratorsAndUpdateCompilation(CreateCompilation(files), out _, out var diagnostics);

    Assert.That(diagnostics.IsEmpty, Is.True);
    
    myGeneratedFiles = driver.GetRunResult().GeneratedTrees
      .Select(tree => tree.ToGeneratedFile())
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