using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models.TopLevel;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;


[Generator]
public class ManualfacGenerator : IIncrementalGenerator
{
  private const string BuildProperty = "build_property";
  private const string NamingStyleProperty = $"{BuildProperty}.DependenciesNamingStyle";


  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    var combined = context.AnalyzerConfigOptionsProvider.Combine(context.CompilationProvider);
    context.RegisterSourceOutput(combined, (productionContext, tuple) =>
    {
      var manualfacContext = new ManualfacContext(productionContext, tuple.Left, tuple.Right);
      DoGeneration(manualfacContext);
    });
  }

  private void DoGeneration(ManualfacContext context)
  {
    var storage = new ComponentsStorage();
    storage.FillComponents(context.Compilation);
    
    GenerateDependenciesPart(storage.AllComponents, context);
    GenerateContainerBuilder(storage, context.Compilation, context.ProductionContext);

    if (ShouldGenerateResolverFor(context.Compilation.Assembly))
    {
      GenerateContainerInitialization(storage, context.Compilation, context.ProductionContext);
      GenerateContainerGenericResolver(storage, context.Compilation, context.ProductionContext); 
    }
  }

  private static bool ShouldGenerateResolverFor(IAssemblySymbol symbol) => 
    symbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "GenerateResolverAttribute");

  private static void GenerateDependenciesPart(
    IReadOnlyList<IComponent> components, ManualfacContext context)
  {
    NamingStyle namingStyle = DefaultNamingStyle.Instance;
    if (context.Provider.GlobalOptions.TryGetValue(NamingStyleProperty, out var explicitlySetStyle))
    {
      namingStyle = NamingStyle.ParseOrDefault(explicitlySetStyle);
    }
    
    foreach (var componentInfo in components)
    {
      var assembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (!SymbolEqualityComparer.Default.Equals(assembly, context.Compilation.Assembly)) continue;
      
      var sb = new StringBuilder();
      componentInfo.ToGeneratedFileModel(namingStyle).GenerateInto(sb, 0);

      context.ProductionContext.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
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