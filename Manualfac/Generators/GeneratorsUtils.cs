using System.Text;

namespace Manualfac.Generators;

internal readonly unsafe struct OpenCloseStringBuilderOperation : IDisposable
{
  private readonly StringBuilder myStringBuilder;
  private readonly Action<StringBuilder> myCloseAction;
  private readonly int* myIndent;

  
  public OpenCloseStringBuilderOperation(
    StringBuilder sb, 
    Action<StringBuilder> openAction, 
    Action<StringBuilder> closeAction,
    int* indent)
  {
    myIndent = indent;
    myStringBuilder = sb;
    myCloseAction = closeAction;
    openAction(sb);
    myStringBuilder.AppendNewLine();
    *indent += 1;
  }
  
  
  public void Dispose()
  {
    myStringBuilder.AppendNewLine();
    *myIndent -= 1;
    myCloseAction(myStringBuilder);
  }
}

internal static unsafe class StringBuilderCookies
{
  public static OpenCloseStringBuilderOperation CurlyBraces(StringBuilder sb, int* indent) =>
    new(
      sb,
      sb => sb.AppendIndent(*indent).AppendOpenCurlyBrace(),
      sb => sb.AppendIndent(*indent).AppendClosedCurlyBrace(),
      indent
    );

  public static OpenCloseStringBuilderOperation DefaultBraces(
    StringBuilder sb, int* indent, bool appendStartIndent = false, bool appendEndIndent = false)
  {
    void OpenAction(StringBuilder sb)
    {
      if (appendStartIndent) sb.AppendIndent(*indent);

      sb.AppendOpenBracket();
    }

    void CloseAction(StringBuilder sb)
    {
      if (appendEndIndent) sb.AppendIndent(*indent);

      sb.AppendClosedBrace();
    }

    return new OpenCloseStringBuilderOperation(sb, OpenAction, CloseAction, indent);
  }

  public static OpenCloseStringBuilderOperation Lock(StringBuilder sb, string objectName, int* indent)
  {
    sb.AppendIndent(*indent).Append("lock (").Append(objectName).Append(")").AppendNewLine();
    return CurlyBraces(sb, indent);
  }
}