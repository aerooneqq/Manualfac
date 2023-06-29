using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ContainerInitializationGenerator
{
  public static void Generate(ComponentsStorage storage, GeneratorExecutionContext context)
  {
    if (storage.BaseToOverrides.Count == 0) return;
    
    var sb = new StringBuilder();
    new GeneratedContainerInitializerModel(storage, context.Compilation.Assembly).GenerateInto(sb, 0);
    
    context.AddSource($"{context.Compilation.Assembly.Name}", sb.ToString());
  }
}