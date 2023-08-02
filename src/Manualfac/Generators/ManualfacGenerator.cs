using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Components.Caches;
using Manualfac.Generators.Models.TopLevel;
using Manualfac.Generators.Util;
using Manualfac.Generators.Util.Naming;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

[Generator]
public class ManualfacGenerator : IIncrementalGenerator
{
  private const string BuildProperty = "build_property";
  private const string DependencySuffixProperty = $"{BuildProperty}.{Constants.Manualfac}DependencySuffix";
  private const string DependencyPrefixProperty = $"{BuildProperty}.{Constants.Manualfac}DependencyPrefix";


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
    var symbols = ManualfacSymbols.CreateManualfacSymbolsFrom(context.Compilation);
    var storage = new ComponentsStorage(symbols, context.Compilation);

    storage.FillComponents();

    GenerateDependenciesPart(storage.AllComponents, context);
    GenerateContainerBuilder(storage, context.Compilation, context.ProductionContext);

    if (ShouldGenerateResolverFor(context.Compilation.Assembly, symbols))
    {
      GenerateContainerInitialization(storage, context.Compilation, context.ProductionContext);
      GenerateContainerGenericResolver(storage, context.Compilation, context.ProductionContext);
    }
  }

  private static bool ShouldGenerateResolverFor(IAssemblySymbol symbol, ManualfacSymbols symbols) =>
    symbol.GetAttributes()
      .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, symbols.GenerateResolverAttribute));

  private static void GenerateDependenciesPart(
    IReadOnlyList<IComponent> components, ManualfacContext context)
  {
    var namingStyle = ParseNamingStyle(context);
    foreach (var componentInfo in components)
    {
      var assembly = componentInfo.ComponentSymbol.ContainingAssembly;
      if (!SymbolEqualityComparer.Default.Equals(assembly, context.Compilation.Assembly)) continue;

      var sb = new StringBuilder();
      componentInfo.ToGeneratedFileModel(namingStyle).GenerateInto(sb, 0);

      context.ProductionContext.AddSource($"{componentInfo.TypeShortName}.g", sb.ToString());
    }
  }

  private static NamingStyle ParseNamingStyle(ManualfacContext context)
  {
    var prefix = string.Empty;
    var suffix = string.Empty;
    var options = context.Provider.GlobalOptions;

    if (options.TryGetValue(DependencyPrefixProperty, out var setPrefix))
    {
      prefix = setPrefix;
    }

    if (options.TryGetValue(DependencySuffixProperty, out var setSuffix))
    {
      suffix = setSuffix;
    }

    return new PrefixSuffixNamingStyle(prefix, suffix);
  }

  private static void GenerateContainerBuilder(
    ComponentsStorage storage, Compilation compilation, SourceProductionContext context)
  {
    var compilationAssembly = compilation.Assembly;
    var sortedComponents = ComponentsTopologicalSorter.Sort(
      storage.AllComponents, static component => component.ResolveConcreteDependencies());

    foreach (var componentInfo in sortedComponents)
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