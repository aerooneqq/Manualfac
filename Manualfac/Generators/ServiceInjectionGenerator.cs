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
    var storage = new ComponentInfoStorage();
    storage.FillComponents(context);
    GenerateDependenciesPart(storage.Components, context);
  }

  private void GenerateDependenciesPart(IReadOnlyList<ComponentInfo> components, GeneratorExecutionContext context)
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
}