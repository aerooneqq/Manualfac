using System.Diagnostics;
using Manualfac.Analysis;
using Manualfac.Components.Caches;
using Manualfac.Components.Dependencies;
using Manualfac.Exceptions;
using Manualfac.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Components;

internal class ComponentsStorage
{
  private readonly ManualfacContext myContext;
  private readonly ComponentAndNonComponentSymbols mySymbolsCache;
  private readonly ComponentsCache myCache;
  private readonly OverridesCache myOverridesCache;
  private readonly ManualfacSymbols myManualfacSymbols;
  private readonly CompilationAssemblyAnalyzer myCompilationAssemblyAnalyzer;


  public IReadOnlyDictionary<IComponent, IComponent> BaseToOverrides => myOverridesCache.BaseToOverrides;
  public IReadOnlyList<IComponent> AllComponents => myCache.AllComponents;

  public IReadOnlyDictionary<INamedTypeSymbol, List<IComponent>> InterfacesToComponents =>
    myCache.InterfacesToComponents;


  public ComponentsStorage(ManualfacSymbols symbols, ManualfacContext context)
  {
    myManualfacSymbols = symbols;
    myContext = context;

    mySymbolsCache = new ComponentAndNonComponentSymbols(myManualfacSymbols);
    myCache = new ComponentsCache(myManualfacSymbols);
    myOverridesCache = new OverridesCache();
    myCompilationAssemblyAnalyzer = new CompilationAssemblyAnalyzer(symbols, this);
  }


  public bool TryFillComponents()
  {
    AllModulesVisitor.Visit(myContext.Compilation, false, module =>
    {
      ProcessModule(module);
      return false;
    });
    
    myCache.AdjustInterfaceImplementations(this.AdjustComponent);

    if (myCompilationAssemblyAnalyzer.ContainsErrors(myContext)) return false;

    ProcessModule(myContext.Compilation.SourceModule);

    myCache.AdjustInterfaceImplementations(this.AdjustComponent);
    myCache.SortByBeforeAfterRelation();

    return true;
  }

  private void ProcessModule(IModuleSymbol module)
  {
    foreach (var componentType in module.GetTypes().Where(mySymbolsCache.CheckIfManualfacComponent))
    {
      var visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
      ToComponentInfo(componentType, visited);
    }
  }

  public bool CheckIfComponent(INamedTypeSymbol symbol) => mySymbolsCache.CheckIfManualfacComponent(symbol);

  private IComponent? ToComponentInfo(INamedTypeSymbol componentSymbol, ISet<INamedTypeSymbol> visited)
  {
    if (myCache.TryGetExistingComponent(componentSymbol) is { } existingComponent) return existingComponent;

    if (visited.Contains(componentSymbol)) throw new CyclicDependencyException();

    if (SymbolEqualityComparer.Default.Equals(componentSymbol.ContainingAssembly, myContext.Compilation.Assembly) &&
        !componentSymbol.CheckIfPartialClass())
    {
      return null;
    }

    if (!mySymbolsCache.CheckIfManualfacComponent(componentSymbol))
    {
      throw new TypeSymbolIsNotManualfacComponentException(componentSymbol);
    }

    visited.Add(componentSymbol);

    var componentsDepsByLevels = ExtractComponentsDependencies(componentSymbol, visited);
    var baseComponent = TryFindBaseComponent(componentSymbol, visited);

    var manualInitialization = componentSymbol
      .GetAttributes()
      .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, myManualfacSymbols.ManualInitialization));

    var createdComponent = new Component(componentSymbol, componentsDepsByLevels, baseComponent, manualInitialization);

    if (TryFindOverridenComponent(componentSymbol, visited) is { } overridenComponent)
    {
      myOverridesCache.AddOverride(createdComponent, overridenComponent);
    }

    myCache.UpdateExistingComponent(componentSymbol, createdComponent);

    AddToInterfacesToImplementationsMap(createdComponent);

    return createdComponent;
  }

  private IReadOnlyList<ComponentDependencyDescriptor> ExtractComponentsDependencies(
    INamedTypeSymbol componentSymbol,
    ISet<INamedTypeSymbol> visited)
  {
    var alreadyAddedDependencySymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

    var dependencies = new List<ComponentDependencyDescriptor>();
    foreach (var dependencyTypes in ExtractDependenciesByLevels(componentSymbol))
    {
      var modifier = ExtractAccessModifierOrDefault(dependencyTypes);

      foreach (var dependencySymbol in dependencyTypes.Skip(1))
      {
        if (alreadyAddedDependencySymbols.Contains(dependencySymbol))
        {
          throw new DuplicatedDependencyException(componentSymbol, dependencySymbol);
        }

        if (TryGetDependency(dependencySymbol, visited) is { } dependency)
        {
          dependencies.Add(new ComponentDependencyDescriptor(dependency, modifier));
        }

        alreadyAddedDependencySymbols.Add(dependencySymbol);
      }
    }

    return dependencies;
  }

  private IComponentDependency? TryGetDependency(INamedTypeSymbol dependencySymbol, ISet<INamedTypeSymbol> visited) =>
    dependencySymbol.TypeKind switch
    {
      TypeKind.Class => ToComponentInfo(dependencySymbol, visited) switch
      {
        { } dependencyComponent => new ConcreteComponentDependency(dependencyComponent),
        _ => null
      },
      TypeKind.Interface when dependencySymbol.IsGenericEnumerable() =>
        new CollectionDependency(dependencySymbol, this),
      TypeKind.Interface => new NonCollectionInterfaceDependency(dependencySymbol, this),
      _ => null
    };

  private void AddToInterfacesToImplementationsMap(IComponent component)
  {
    if (component.Symbol.TypeKind is not TypeKind.Class) return;

    var allInterfaces = ExtractInterfaces(component.Symbol);

    if (allInterfaces.Count == 0) return;

    foreach (var @interface in allInterfaces)
    {
      myCache.AddInterfaceImplementation(@interface, component);
    }
  }

  private IComponent? TryFindBaseComponent(INamedTypeSymbol symbol, ISet<INamedTypeSymbol> visited)
  {
    if (symbol.BaseType is not { } baseType ||
        !mySymbolsCache.CheckIfManualfacComponent(baseType))
    {
      return null;
    }

    return ToComponentInfo(baseType, visited);
  }

  private IComponent? TryFindOverridenComponent(INamedTypeSymbol symbol, ISet<INamedTypeSymbol> visited)
  {
    var baseSymbols = symbol.GetAttributesTypeArguments(myManualfacSymbols.OverridesAttribute);

    if (baseSymbols.Count == 0) return null;

    if (baseSymbols.Count != 1)
    {
      throw new TooManyOverridesException(symbol, baseSymbols);
    }

    var baseSymbol = baseSymbols.First();
    if (baseSymbol.TypeKind != TypeKind.Class)
    {
      throw new OverrideMustBeClassException(symbol, baseSymbol);
    }

    if (!mySymbolsCache.CheckIfManualfacComponent(baseSymbol))
    {
      throw new CanNotOverrideNonComponentException(symbol, baseSymbol);
    }

    var baseComponent = ToComponentInfo(baseSymbol, visited);

    return baseComponent;
  }

  private IReadOnlyList<INamedTypeSymbol> ExtractInterfaces(INamedTypeSymbol symbol)
  {
    var asAttributes = symbol.GetAttributesTypeArguments(myManualfacSymbols.AsAttributeBase);

    return asAttributes.Count == 0 ? symbol.AllInterfaces : asAttributes;
  }

  private IReadOnlyList<IReadOnlyList<INamedTypeSymbol>> ExtractDependenciesByLevels(INamedTypeSymbol symbol)
  {
    return symbol.GetAttributesTypeArgumentsByLevels(myManualfacSymbols.DependsOnAttributeBase);
  }

  private AccessModifier ExtractAccessModifierOrDefault(IReadOnlyList<ITypeSymbol> dependencyAttributeTypeArgs)
  {
    Debug.Assert(dependencyAttributeTypeArgs.Count > 1);
    var first = dependencyAttributeTypeArgs.First();

    return first.Name switch
    {
      nameof(AccessModifier.Internal) => AccessModifier.Internal,
      nameof(AccessModifier.Protected) => AccessModifier.Protected,
      nameof(AccessModifier.Private) => AccessModifier.Private,
      nameof(AccessModifier.Public) => AccessModifier.Public,
      nameof(AccessModifier.PrivateProtected) => AccessModifier.PrivateProtected,
      nameof(AccessModifier.ProtectedInternal) => AccessModifier.ProtectedInternal,
      _ => throw new ArgumentOutOfRangeException(first.Name)
    };
  }
}

internal static class ComponentsStorageExtensions
{
  public static IComponent AdjustComponent(this ComponentsStorage storage, IComponent component)
  {
    var current = component;
    while (storage.BaseToOverrides.TryGetValue(current, out var @override))
    {
      current = @override;
    }

    return current;
  }
}