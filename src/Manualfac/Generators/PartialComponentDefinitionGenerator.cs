using System.Text;
using Manualfac.Generators.Components;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static class PartialComponentDefinitionGenerator
{
  public static void GenerateDependenciesPart(IComponentInfo componentInfo, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();
    componentInfo.ToGeneratedFileModel().GenerateInto(sb, 0);
    
    context.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
  }
}