using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models;
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
      if (!SymbolEqualityComparer.Default.Equals(assembly, context.Compilation.Assembly)) continue;
      
      var sb = new StringBuilder();
      componentInfo.ToGeneratedFileModel().GenerateInto(sb, 0);
    
      context.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
    }
  }

  private static void GenerateContainerBuilder(ComponentsStorage storage, GeneratorExecutionContext context)
  {
    var compilationAssembly = context.Compilation.Assembly;
    foreach (var componentInfo in ComponentsTopologicalSorter.Sort(storage.AllComponents))
    {
      var componentAssembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (!SymbolEqualityComparer.Default.Equals(compilationAssembly, componentAssembly)) continue;
      
      var sb = new StringBuilder();
      new GeneratedComponentContainerModel(componentInfo).GenerateInto(sb, 0);
    
      context.AddSource($"{componentInfo.CreateContainerName()}.g", sb.ToString());
    }
  }

  private static void GenerateContainerInitialization(ComponentsStorage storage, GeneratorExecutionContext context)
  {
    if (storage.BaseToOverrides.Count == 0) return;
    
    var sb = new StringBuilder();
    new GeneratedContainerInitializerModel(storage, context.Compilation.Assembly).GenerateInto(sb, 0);
    
    context.AddSource($"{context.Compilation.Assembly.Name}", sb.ToString());
  }
}