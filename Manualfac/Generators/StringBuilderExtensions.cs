using System.Text;

namespace Manualfac.Generators;

public static class StringBuilderExtensions
{
  public static StringBuilder AppendNewLine(this StringBuilder sb) => sb.Append('\n');
  public static StringBuilder AppendSemicolon(this StringBuilder sb) => sb.Append(';');
  public static StringBuilder AppendSpace(this StringBuilder sb) => sb.Append(' ');
  public static StringBuilder AppendComma(this StringBuilder sb) => sb.Append(',');
  public static StringBuilder AppendOpenBracket(this StringBuilder sb) => sb.Append('(');
  public static StringBuilder AppendClosedBracket(this StringBuilder sb) => sb.Append(')');
  public static StringBuilder AppendOpenCurlyBracket(this StringBuilder sb) => sb.Append('{');
  public static StringBuilder AppendClosedCurlyBracket(this StringBuilder sb) => sb.Append('}');
  public static StringBuilder AppendEq(this StringBuilder sb) => sb.Append('=');

  public static StringBuilder AppendTab(this StringBuilder sb) => sb.AppendSpace().AppendSpace();
}