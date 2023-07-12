using Microsoft.CodeAnalysis;

namespace TestCore;

public readonly record struct GeneratedFile(string Name, string Text);

public static class SyntaxTreeExtensions
{
  public static GeneratedFile ToGeneratedFile(this SyntaxTree tree) =>
    new(
      tree.FilePath[(tree.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..],
      tree.GetText().ToString().ReplaceRn()
    );
}

public static class StringExtensions
{
  public static string ReplaceRn(this string text) => text.Replace("\r\n", "\n");
}