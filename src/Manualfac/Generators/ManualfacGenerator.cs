using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models.TopLevel;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

[Generator]
public class ManualfacGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    context.RegisterSourceOutput(context.CompilationProvider, DoGeneration);
  }

  private void DoGeneration(SourceProductionContext productionContext, Compilation compilation)
  {
    var storage = new ComponentsStorage();
    storage.FillComponents(compilation);
    
    GenerateDependenciesPart(storage.AllComponents, compilation, productionContext);
    GenerateContainerBuilder(storage, compilation, productionContext);

    if (ShouldGenerateResolverFor(compilation.Assembly))
    {
      GenerateContainerInitialization(storage, compilation, productionContext);
      GenerateContainerGenericResolver(storage, compilation, productionContext); 
    }
  }

  private static bool ShouldGenerateResolverFor(IAssemblySymbol symbol) => 
    symbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "GenerateResolverAttribute");

  private static void GenerateDependenciesPart(
    IReadOnlyList<IComponent> components, Compilation compilation, SourceProductionContext context)
  {
    foreach (var componentInfo in components)
    {
      var assembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (!SymbolEqualityComparer.Default.Equals(assembly, compilation.Assembly)) continue;
      
      var sb = new StringBuilder();
      componentInfo.ToGeneratedFileModel().GenerateInto(sb, 0);
    
      context.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
    }
  }

  private static void GenerateContainerBuilder(
    ComponentsStorage storage, Compilation compilation, SourceProductionContext context)
  {
    var compilationAssembly = compilation.Assembly;
    foreach (var componentInfo in ComponentsTopologicalSorter.Sort(storage.AllComponents))
    {
      var componentAssembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (!SymbolEqualityComparer.Default.Equals(compilationAssembly, componentAssembly)) continue;
      
      var sb = new StringBuilder();
      new GeneratedComponentContainerModel(componentInfo).GenerateInto(sb, 0);
    
      context.AddSource($"{componentInfo.CreateContainerName()}.g", sb.ToString());
    }
  }

  private static void GenerateContainerInitialization(
    ComponentsStorage storage, Compilation compilation, SourceProductionContext context)
  {
    if (storage.BaseToOverrides.Count == 0) return;
    
    var sb = new StringBuilder();
    new GeneratedContainerInitializerModel(storage, compilation.Assembly).GenerateInto(sb, 0);
    
    context.AddSource($"{compilation.Assembly.Name}", sb.ToString());
  }

  private static void GenerateContainerGenericResolver(
    ComponentsStorage storage, Compilation compilation, SourceProductionContext context)
  {
    var sb = new StringBuilder();
    new GeneratedContainerResolverModel(storage, compilation.Assembly).GenerateInto(sb, 0);
    
    context.AddSource($"{compilation.Assembly.Name}Resolver", sb.ToString());
  }
}