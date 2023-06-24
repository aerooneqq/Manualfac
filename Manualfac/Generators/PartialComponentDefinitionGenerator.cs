using System.Text;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static unsafe class PartialComponentDefinitionGenerator
{
  public static void GenerateDependenciesPart(ComponentInfo componentInfo, GeneratorExecutionContext context)
  {
    var sb = WriteUsings(componentInfo, new StringBuilder()).AppendNewLine();

    OpenCloseStringBuilderOperation? namespaceCookie = null;
    var indent = 0;

    try
    {
      if (componentInfo.Namespace is { })
      {
        sb.Append("namespace").AppendSpace().Append(componentInfo.Namespace).AppendSpace().AppendNewLine();
        namespaceCookie = StringBuilderCookies.CurlyBraces(sb, &indent);
      }
      
      sb.AppendIndent(indent).Append("public partial class ").Append(componentInfo.ShortName).AppendNewLine();

      using (StringBuilderCookies.CurlyBraces(sb, &indent))
      {
        WriteDependenciesFields(componentInfo, sb, indent);
        sb.AppendNewLine();
        WriteConstructor(componentInfo, sb, &indent);
      }
    }
    finally
    {
      namespaceCookie?.Dispose();
    }
    
    context.AddSource($"{componentInfo.ShortName}.g", sb.ToString());
  }

  private static StringBuilder WriteUsings(ComponentInfo componentInfo, StringBuilder sb)
  {
    foreach (var dependency in componentInfo.Dependencies)
    {
      if (dependency.Namespace is { })
      {
        sb.Append("using ").Append(dependency.Namespace).AppendSemicolon().AppendNewLine(); 
      }
    }

    return sb;
  }
  
  private static StringBuilder WriteDependenciesFields(ComponentInfo componentInfo, StringBuilder sb, int indent)
  {
    foreach (var (dependency, modifier) in componentInfo.OrderedDependencies)
    {
      sb.AppendIndent(indent).Append(modifier.CreateModifierString()).AppendSpace()
        .Append(dependency.ShortName).AppendSpace();
      
      WriteFieldName(dependency, sb, 0);
      
      sb.AppendSemicolon().AppendNewLine();
    }

    return sb;
  }

  private static StringBuilder WriteFieldName(ComponentInfo component, StringBuilder sb, int indent)
  {
    return sb.AppendIndent(indent).Append("my").Append(component.ShortName);
  }

  private static StringBuilder WriteConstructor(ComponentInfo componentInfo, StringBuilder sb, int* indent)
  {
    sb.AppendIndent(*indent).Append("public ").Append(componentInfo.ShortName);

    using (StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      var index = 0;
      foreach (var (dependency, _) in componentInfo.OrderedDependencies)
      {
        sb.AppendIndent(*indent).Append(dependency.ShortName).AppendSpace().Append(GetComponentParamName(index++))
          .AppendComma().AppendNewLine();
      }
      
      if (index > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }
    
    using (StringBuilderCookies.CurlyBraces(sb.AppendNewLine(), indent))
    {
      var index = 0;
      foreach (var (dependency, _) in componentInfo.OrderedDependencies)
      {
        WriteFieldName(dependency, sb, *indent);
        sb.AppendSpace().AppendEq().AppendSpace().Append(GetComponentParamName(index++))
          .AppendSemicolon().AppendNewLine();
      }

      //remove last new line
      sb.Remove(sb.Length - 1, 1);
    }
    
    return sb;

    static string GetComponentParamName(int index) => $"c{index}";
  }
}

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
    new(sb, sb => sb.AppendIndent(*indent).AppendOpenCurlyBrace(), sb => sb.AppendIndent(*indent).AppendClosedCurlyBrace(), indent);

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
}