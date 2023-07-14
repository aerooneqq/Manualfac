using System.Diagnostics;
using Manualfac.Exceptions;
using Manualfac.Generators.Components.Caches;
using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class ComponentsStorage
{
  private readonly ComponentAndNonComponentSymbols mySymbolsCache;
  private readonly ComponentsCache myCache;
  private readonly OverridesCache myOverridesCache;

  
  public IReadOnlyDictionary<IComponent, IComponent> BaseToOverrides => myOverridesCache.BaseToOverrides;
  public IReadOnlyList<IComponent> AllComponents => myCache.AllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IComponent>> InterfacesToComponents => myCache.InterfacesToComponents;


  public ComponentsStorage()
  {
    mySymbolsCache = new ComponentAndNonComponentSymbols();
    myCache = new ComponentsCache();
    myOverridesCache = new OverridesCache();
  }
  
  
  public void FillComponents(Compilation compilation)
  {
    AllModulesVisitor.Visit(compilation, module =>
    {
      foreach (var componentType in module.GetTypes().Where(mySymbolsCache.CheckIfManualfacComponent))
      {
        var visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        ToComponentInfo(componentType, visited, compilation);
      }

      return false;
    });
    
    myCache.AdjustInterfaceImplementations(this.AdjustComponent);
  }
  
  private IComponent ToComponentInfo(
    INamedTypeSymbol componentSymbol, ISet<INamedTypeSymbol> visited, Compilation compilation)
  {
    if (myCache.TryGetExistingComponent(componentSymbol) is { } existingComponent) return existingComponent;
    if (visited.Contains(componentSymbol)) throw new CyclicDependencyException();

    if (!mySymbolsCache.CheckIfManualfacComponent(componentSymbol))
    {
      throw new TypeSymbolIsNotManualfacComponentException(componentSymbol);
    }

    visited.Add(componentSymbol);

    var componentsDepsByLevels = ExtractComponentsDependencies(componentSymbol, visited, compilation);
    var baseComponent = TryFindBaseComponent(componentSymbol, visited, compilation);
    var createdComponent = new Component(componentSymbol, componentsDepsByLevels, baseComponent);

    if (TryFindOverridenComponent(componentSymbol, visited, compilation) is { } overridenComponent)
    {
      myOverridesCache.AddOverride(createdComponent, overridenComponent);
    }
    
    myCache.UpdateExistingComponent(componentSymbol, createdComponent);
    
    AddToInterfacesToImplementationsMap(createdComponent);
    
    return createdComponent;
  }

  private IReadOnlyList<ComponentDependencyDescriptor> ExtractComponentsDependencies(
    INamedTypeSymbol componentSymbol, 
    ISet<INamedTypeSymbol> visited,
    Compilation compilation)
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

        if (TryGetDependency(dependencySymbol, visited, compilation) is { } dependency)
        {
          dependencies.Add(new ComponentDependencyDescriptor(dependency, modifier));
        }

        alreadyAddedDependencySymbols.Add(dependencySymbol);
      }
    }

    return dependencies;
  }

  private IComponentDependency? TryGetDependency(
    INamedTypeSymbol dependencySymbol, ISet<INamedTypeSymbol> visited, Compilation compilation)
  {
    if (dependencySymbol.TypeKind == TypeKind.Class)
    {
      var dependencyComponent = ToComponentInfo(dependencySymbol, visited, compilation);
      return new ConcreteComponentDependency(dependencyComponent);
    }

    if (dependencySymbol.TypeKind == TypeKind.Interface)
    {
      if (dependencySymbol.IsGenericEnumerable())
      {
        return new CollectionDependency(dependencySymbol, this);
      }

      return new NonCollectionInterfaceDependency(dependencySymbol, this);
    }

    return null;
  }

  private void AddToInterfacesToImplementationsMap(IComponent component)
  {
    if (component.ComponentSymbol.TypeKind is not TypeKind.Class) return;

    var allInterfaces = ExtractInterfaces(component.ComponentSymbol);
    if (allInterfaces.Count == 0) return;

    foreach (var @interface in allInterfaces)
    {
      myCache.AddInterfaceImplementation(@interface, component);
    }
  }

  private IComponent? TryFindBaseComponent(
    INamedTypeSymbol symbol, ISet<INamedTypeSymbol> visited, Compilation context)
  {
    if (symbol.BaseType is not { } baseType ||
        !mySymbolsCache.CheckIfManualfacComponent(baseType))
    {
      return null;
    }

    return ToComponentInfo(baseType, visited, context);
  }

  private IComponent? TryFindOverridenComponent(
    INamedTypeSymbol symbol, ISet<INamedTypeSymbol> visited, Compilation context)
  {
    var overridesAttributes = ExtractAttributeByNameWithTypeArgs(symbol, Constants.OverridesAttribute);
    var baseSymbols = overridesAttributes.SelectMany(pair => pair).ToList();
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

    var baseComponent = ToComponentInfo(baseSymbol, visited, context);
    return baseComponent;
  }

  private IReadOnlyList<INamedTypeSymbol> ExtractInterfaces(INamedTypeSymbol symbol)
  {
    var asAttributes = ExtractAttributeByNameWithTypeArgs(symbol, Constants.AsAttribute).ToList();
    if (asAttributes.Count == 0)
    {
      return symbol.AllInterfaces;
    }

    return asAttributes.SelectMany(pair => pair).ToList();
  }

  private static IReadOnlyList<IReadOnlyList<INamedTypeSymbol>> ExtractDependenciesByLevels(INamedTypeSymbol symbol)
  {
    return ExtractAttributeByNameWithTypeArgs(symbol, Constants.DependsOnAttribute).ToList();
  }

  private static IReadOnlyList<IReadOnlyList<INamedTypeSymbol>> ExtractAttributeByNameWithTypeArgs(
    ISymbol symbol, string attributeName)
  {
    return symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.Name == attributeName)
      .Select(attr => attr.AttributeClass!.TypeArguments.OfType<INamedTypeSymbol>().ToList())
      .ToList();
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