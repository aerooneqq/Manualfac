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
      
      componentInfo.ToGeneratedClassModel().GenerateInto(sb, indent);
    }
    finally
    {
      namespaceCookie?.Dispose();
    }
    
    context.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
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
}