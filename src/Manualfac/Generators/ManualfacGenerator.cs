using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models;
using Manualfac.Generators.Models.TopLevel;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

[Generator]
public class ManualfacGenerator : ISourceGenerator
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

    if (ShouldGenerateResolverFor(context.Compilation.Assembly))
    {
      GenerateContainerInitialization(storage, context);
      GenerateContainerGenericResolver(storage, context); 
    }
  }

  private static bool ShouldGenerateResolverFor(IAssemblySymbol symbol) => 
    symbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "GenerateResolverAttribute");

  private static void GenerateDependenciesPart(
    IReadOnlyList<IComponent> components, GeneratorExecutionContext context)
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

  private static void GenerateContainerGenericResolver(ComponentsStorage storage, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();
    new GeneratedContainerResolverModel(storage, context.Compilation.Assembly).GenerateInto(sb, 0);
    
    context.AddSource($"{context.Compilation.Assembly.Name}Resolver", sb.ToString());
  }
}