using System.Text;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static unsafe class PartialComponentDefinitionGenerator
{
  public static void GenerateDependenciesPart(ComponentInfo componentInfo, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();
    componentInfo.ToGeneratedFileModel().GenerateInto(sb, 0);
    
    context.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
  }
}