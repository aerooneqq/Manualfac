using System.Text;

namespace Manualfac.Generators.Util;

internal readonly struct OpenCloseStringBuilderOperation : IDisposable
{
  private const int IndentDelta = 1;
  
  private readonly StringBuilder myStringBuilder;
  private readonly Action<StringBuilder, int> myCloseAction;
  
  
  public int Indent { get; }

  
  public OpenCloseStringBuilderOperation(
    StringBuilder sb, 
    Action<StringBuilder, int> openAction, 
    Action<StringBuilder, int> closeAction,
    int indent)
  {
    myStringBuilder = sb;
    myCloseAction = closeAction;
    openAction(sb, indent);
    myStringBuilder.AppendNewLine();
    Indent = indent + IndentDelta;
  }
  
  
  public void Dispose()
  {
    myStringBuilder.AppendNewLine();
    myCloseAction(myStringBuilder, Indent - IndentDelta);
  }
}

internal static class StringBuilderCookies
{
  public static OpenCloseStringBuilderOperation CurlyBraces(StringBuilder sb, int indent) =>
    new(
      sb,
      (sb, indent) => sb.AppendIndent(indent).AppendOpenCurlyBrace(),
      (sb, indent) => sb.AppendIndent(indent).AppendClosedCurlyBrace(),
      indent
    );

  public static OpenCloseStringBuilderOperation DefaultBraces(
    StringBuilder sb, int indent, bool appendStartIndent = false, bool appendEndIndent = false)
  {
    void OpenAction(StringBuilder sb, int indent)
    {
      if (appendStartIndent) sb.AppendIndent(indent);

      sb.AppendOpenBracket();
    }

    void CloseAction(StringBuilder sb, int indent)
    {
      if (appendEndIndent) sb.AppendIndent(indent);

      sb.AppendClosedBrace();
    }

    return new OpenCloseStringBuilderOperation(sb, OpenAction, CloseAction, indent);
  }

  public static OpenCloseStringBuilderOperation Lock(StringBuilder sb, string objectName, int indent)
  {
    sb.AppendIndent(indent).Append("lock (").Append(objectName).Append(")").AppendNewLine();
    return CurlyBraces(sb, indent);
  }

  public static void If(StringBuilder sb, int indent, string condition, Action<StringBuilder, int> ifBodyGenerator)
  {
    sb.AppendIndent(indent).Append("if (").Append(condition).Append(')').AppendNewLine();
    using var cookie = CurlyBraces(sb, indent);
    ifBodyGenerator(sb, cookie.Indent);
  }
}