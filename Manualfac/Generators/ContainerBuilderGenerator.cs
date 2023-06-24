using System.Text;
using Manualfac.Generators.Models;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static class ContainerBuilderGenerator
{
  public static void GenerateContainerInitialization(
    ComponentInfoStorage componentsStorage, GeneratorExecutionContext context)
  {
    var compilationAssembly = context.Compilation.Assembly;
    foreach (var componentInfo in componentsStorage.GetInTopologicalOrder())
    {
      var componentAssembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (SymbolEqualityComparer.Default.Equals(compilationAssembly, componentAssembly))
      {
        GenerateComponentBuilderClass(componentInfo, context);
      }
    }
  }

  private static void GenerateComponentBuilderClass(ComponentInfo componentInfo, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();
    new GeneratedComponentContainerModel(componentInfo).GenerateInto(sb, 0);
    
    context.AddSource($"{componentInfo.CreateContainerName()}.g", sb.ToString());
  }
}