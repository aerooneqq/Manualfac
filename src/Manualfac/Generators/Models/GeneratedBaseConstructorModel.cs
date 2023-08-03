using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedBaseConstructorModel(IReadOnlyList<string> baseConstructorArgs) : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    if (baseConstructorArgs.Count == 0) return;

    sb.Append(" : base");
    using var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true);
    foreach (var argument in baseConstructorArgs)
    {
      sb.AppendIndent(cookie.Indent).Append(argument).AppendComma().AppendNewLine();
    }

    if (baseConstructorArgs.Count > 0)
    {
      //remove last new line and comma
      sb.Remove(sb.Length - 2, 2);
    }
  }
}