using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static class ContainerBuilderGenerator
{
  public static void GenerateContainerInitialization(
    ComponentsStorage componentsStorage, GeneratorExecutionContext context)
  {
    var compilationAssembly = context.Compilation.Assembly;
    foreach (var componentInfo in ComponentsTopologicalSorter.Sort(componentsStorage.AllComponents))
    {
      var componentAssembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (SymbolEqualityComparer.Default.Equals(compilationAssembly, componentAssembly))
      {
        GenerateComponentBuilderClass(componentInfo, context);
      }
    }
  }

  private static void GenerateComponentBuilderClass(IConcreteComponent concreteComponent, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();
    new GeneratedComponentContainerModel(concreteComponent).GenerateInto(sb, 0);
    
    context.AddSource($"{concreteComponent.CreateContainerName()}.g", sb.ToString());
  }
}