using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedBaseConstructorModel : IGeneratedModel
{
  private readonly IReadOnlyList<string> myBaseConstructorArgs;

  
  public GeneratedBaseConstructorModel(IReadOnlyList<string> baseConstructorArgs)
  {
    myBaseConstructorArgs = baseConstructorArgs;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    if (myBaseConstructorArgs.Count == 0) return;
    
    sb.Append(" : base");
    using var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true);
    foreach (var argument in myBaseConstructorArgs)
    {
      sb.AppendIndent(cookie.Indent).Append(argument).AppendComma().AppendNewLine();
    }

    if (myBaseConstructorArgs.Count > 0)
    {
      //remove last new line and comma
      sb.Remove(sb.Length - 2, 2);
    }
  }
}