using System.Text;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static class PartialComponentDefinitionGenerator
{
  public static void GenerateDependenciesPart(ComponentInfo componentInfo, GeneratorExecutionContext context)
  {
    var sb = WriteUsings(componentInfo, new StringBuilder()).AppendNewLine();

    OpenCloseStringBuilderOperation? namespaceCookie = null;

    try
    {
      if (componentInfo.Namespace is { })
      {
        sb.Append("namespace").AppendSpace().Append(componentInfo.Namespace).AppendSpace().AppendNewLine();
        namespaceCookie = StringBuilderCookies.CurlyBraces(sb);
      }
      
      sb.Append("public partial class ").Append(componentInfo.ShortName).AppendNewLine();

      using (StringBuilderCookies.CurlyBraces(sb))
      {
        WriteDependenciesFields(componentInfo, sb);
        sb.AppendNewLine();
        WriteConstructor(componentInfo, sb);
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
  
  private static StringBuilder WriteDependenciesFields(ComponentInfo componentInfo, StringBuilder sb)
  {
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      sb = sb.Append("private readonly ").Append(dependency.ShortName).AppendSpace();
      sb = WriteFieldName(dependency, sb);
      
      sb = sb.AppendSemicolon().AppendNewLine();
    }

    return sb;
  }

  private static StringBuilder WriteFieldName(ComponentInfo component, StringBuilder sb)
  {
    return sb.Append("my").Append(component.ShortName);
  }

  private static StringBuilder WriteConstructor(ComponentInfo componentInfo, StringBuilder sb)
  {
    sb.Append("public ").Append(componentInfo.ShortName);

    using (StringBuilderCookies.DefaultBraces(sb))
    {
      var index = 0;
      foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
      {
        sb.Append(dependency.ShortName).AppendSpace().Append(GetComponentParamName(index++)).AppendComma()
          .AppendSpace();
      }
    
      if (index > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }
    
    using (StringBuilderCookies.CurlyBraces(sb))
    {
      var index = 0;
      foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
      {
        WriteFieldName(dependency, sb);
        sb.AppendSpace().AppendEq().Append(GetComponentParamName(index++)).AppendSemicolon().AppendNewLine();
      }
    }
    
    return sb;

    static string GetComponentParamName(int index) => $"c{index}";
  }
}

internal readonly struct OpenCloseStringBuilderOperation : IDisposable
{
  private readonly StringBuilder myStringBuilder;
  private readonly Action<StringBuilder> myCloseAction;

  public OpenCloseStringBuilderOperation(
    StringBuilder sb, 
    Action<StringBuilder> openAction, 
    Action<StringBuilder> closeAction)
  {
    myStringBuilder = sb;
    myCloseAction = closeAction;
    openAction(sb);
    myStringBuilder.AppendNewLine();
  }
  
  
  public void Dispose()
  {
    myStringBuilder.AppendNewLine();
    myCloseAction(myStringBuilder);
  }
}

internal static class StringBuilderCookies
{
  public static OpenCloseStringBuilderOperation CurlyBraces(StringBuilder sb) =>
    new(sb, static sb => sb.AppendOpenCurlyBrace(), static sb => sb.AppendClosedCurlyBrace());

  public static OpenCloseStringBuilderOperation DefaultBraces(StringBuilder sb) =>
    new(sb, static sb => sb.AppendOpenBracket(), static sb => sb.AppendClosedBrace());
}