using Manualfac.Generators.Components;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

[Generator]
public class ServiceInjectionGenerator : ISourceGenerator
{
  public void Initialize(GeneratorInitializationContext context)
  {
  }

  public void Execute(GeneratorExecutionContext context)
  {
    var storage = new ComponentsStorage();
    storage.FillComponents(context);
    
    GenerateDependenciesPart(storage.AllComponents, context);
    GenerateContainerBuilder(storage, context);
    GenerateContainerInitialization(storage, context);
  }

  private static void GenerateDependenciesPart(
    IReadOnlyList<IConcreteComponent> components, GeneratorExecutionContext context)
  {
    foreach (var componentInfo in components)
    {
      var assembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (SymbolEqualityComparer.Default.Equals(assembly, context.Compilation.Assembly))
      {
        PartialComponentDefinitionGenerator.GenerateDependenciesPart(componentInfo, context);
      }
    }
  }

  private static void GenerateContainerBuilder(ComponentsStorage storage, GeneratorExecutionContext context)
  {
    ContainerBuilderGenerator.GenerateContainerInitialization(storage, context);
  }

  private static void GenerateContainerInitialization(ComponentsStorage storage, GeneratorExecutionContext context)
  {
    ContainerInitializationGenerator.Generate(storage, context);
  }
}