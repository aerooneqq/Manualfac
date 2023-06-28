using System.Text;
using Manualfac.Generators.Components;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static class PartialComponentDefinitionGenerator
{
  public static void GenerateDependenciesPart(IConcreteComponent concreteComponent, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();
    concreteComponent.ToGeneratedFileModel().GenerateInto(sb, 0);
    
    context.AddSource($"{concreteComponent.TypeShortName}.g", sb.ToString());
  }
}