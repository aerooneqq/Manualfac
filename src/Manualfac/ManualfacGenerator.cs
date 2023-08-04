using System.Text;
using Manualfac.Components;
using Manualfac.Components.Caches;
using Manualfac.Models.TopLevel;
using Manualfac.Util;
using Manualfac.Util.Naming;
using Microsoft.CodeAnalysis;

namespace Manualfac;

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
    GenerateContainerBuilder(storage, context);

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
    foreach (var component in components)
    {
      var componentAssembly = component.Symbol.ContainingAssembly;
      if (!ShouldGenerateContainerOrDepsParts(componentAssembly, context))
      {
        continue;
      }

      var sb = new StringBuilder();
      component.ToGeneratedFileModel(namingStyle).GenerateInto(sb, 0);

      context.ProductionContext.AddSource($"{component.TypeShortName}.g", sb.ToString());
    }
  }

  private static bool ShouldGenerateContainerOrDepsParts(IAssemblySymbol assembly, ManualfacContext context)
  {
    return SymbolEqualityComparer.Default.Equals(assembly, context.Compilation.Assembly);
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

  private static void GenerateContainerBuilder(ComponentsStorage storage, ManualfacContext context)
  {
    var sortedComponents = ComponentsTopologicalSorter.Sort(
      storage.AllComponents, static component => component.ResolveConcreteDependencies());

    foreach (var componentInfo in sortedComponents)
    {
      var componentAssembly = componentInfo.Symbol.ContainingAssembly;
      if (!ShouldGenerateContainerOrDepsParts(componentAssembly, context))
      {
        continue;
      }

      var sb = new StringBuilder();
      new GeneratedContainerModel(componentInfo).GenerateInto(sb, 0);

      context.ProductionContext.AddSource($"{componentInfo.CreateContainerName()}.g", sb.ToString());
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