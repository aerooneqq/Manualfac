using System.Text;

namespace Manualfac.Util;

public static class StringBuilderExtensions
{
  public static StringBuilder AppendNewLine(this StringBuilder sb) => sb.Append('\n');
  public static StringBuilder AppendSemicolon(this StringBuilder sb) => sb.Append(';');
  public static StringBuilder AppendSpace(this StringBuilder sb) => sb.Append(' ');
  public static StringBuilder AppendComma(this StringBuilder sb) => sb.Append(',');
  public static StringBuilder AppendOpenBracket(this StringBuilder sb) => sb.Append('(');
  public static StringBuilder AppendClosedBrace(this StringBuilder sb) => sb.Append(')');
  public static StringBuilder AppendOpenCurlyBrace(this StringBuilder sb) => sb.Append('{');
  public static StringBuilder AppendClosedCurlyBrace(this StringBuilder sb) => sb.Append('}');
  public static StringBuilder AppendEq(this StringBuilder sb) => sb.Append('=');

  public static StringBuilder AppendTab(this StringBuilder sb) => sb.AppendSpace().AppendSpace();

  public static StringBuilder AppendIndent(this StringBuilder sb, int indent)
  {
    for (var i = 0; i < indent; ++i)
    {
      sb.AppendTab();
    }

    return sb;
  }
}