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
      tree.GetText().ToString().ReplaceRn()
    );
}

internal abstract class SourceGeneratorTestExecutorBase<TGenerator> where TGenerator : ISourceGenerator, new()
{
  private readonly string myTestName;

  protected Dictionary<string, string>? GeneratedFiles;
  protected GeneratorDriverRunResult? RunResult;


  protected SourceGeneratorTestExecutorBase(string testName)
  {
    myTestName = testName;
  }

  
  public virtual void ExecuteTest()
  {
    var pathToSources = TestPaths.GetPathToSources(myTestName);
    var files = Directory.EnumerateFiles(pathToSources).Where(path => path.EndsWith(".cs"));
    
    GeneratorDriver driver = CSharpGeneratorDriver.Create(new TGenerator());
    driver = driver.RunGeneratorsAndUpdateCompilation(CreateCompilation(files), out _, out _);
    
    RunResult = driver.GetRunResult();
    
    GeneratedFiles = RunResult.GeneratedTrees
      .Select(tree => tree.ToGeneratedFile())
      .ToDictionary(static file => file.Name, static file => file.Text);
  }
  
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

internal class SourceGeneratorsTestExecutorWithException<TGenerator, TException> : SourceGeneratorTestExecutorBase<TGenerator>
  where TGenerator : ISourceGenerator, new()
  where TException : Exception
{
  public SourceGeneratorsTestExecutorWithException(string testName) : base(testName)
  {
  }
  
  
  public override void ExecuteTest()
  {
    base.ExecuteTest();
    
    Assert.That(RunResult, Is.Not.Null);
    Assert.That(RunResult!.Results.First().Exception is TException);
  }
}

internal static class StringExtensions
{
  public static string ReplaceRn(this string text) => text.Replace("\r\n", "\n");
}