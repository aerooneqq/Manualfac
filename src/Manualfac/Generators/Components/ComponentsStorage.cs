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

  
  public IReadOnlyDictionary<IConcreteComponent, IConcreteComponent> BaseToOverrides => myOverridesCache.BaseToOverrides;
  public IReadOnlyList<IConcreteComponent> AllComponents => myCache.AllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IConcreteComponent>> InterfacesToComponents => myCache.InterfacesToComponents;


  public ComponentsStorage()
  {
    mySymbolsCache = new ComponentAndNonComponentSymbols();
    myCache = new ComponentsCache();
    myOverridesCache = new OverridesCache();
  }
  
  
  public void FillComponents(GeneratorExecutionContext context)
  {
    var modulesQueue = new Queue<IModuleSymbol>();
    var visited = new HashSet<IModuleSymbol>(SymbolEqualityComparer.Default);
    
    modulesQueue.Enqueue(context.Compilation.SourceModule);

    while (modulesQueue.Count != 0)
    {
      var module = modulesQueue.Dequeue();
      visited.Add(module);

      foreach (var componentType in GetComponentTypesFrom(module))
      {
        ToComponentInfo(componentType, new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default), context);
      }

      foreach (var refAsm in module.ReferencedAssemblySymbols)
      {
        foreach (var refAsmModule in refAsm.Modules)
        {
          if (!visited.Contains(refAsmModule))
          {
            modulesQueue.Enqueue(refAsmModule);
          }
        }
      }
    }
  }
  
  private IConcreteComponent ToComponentInfo(
    INamedTypeSymbol componentSymbol, ISet<INamedTypeSymbol> visited, GeneratorExecutionContext context)
  {
    if (myCache.TryGetExistingComponent(componentSymbol) is { } existingComponent) return existingComponent;
    if (visited.Contains(componentSymbol)) throw new CyclicDependencyException();

    if (!mySymbolsCache.CheckIfManualfacComponent(componentSymbol))
    {
      throw new TypeSymbolIsNotManualfacComponentException(componentSymbol);
    }

    visited.Add(componentSymbol);

    var componentsDepsByLevels = ExtractComponentsDependencies(componentSymbol, visited, context);
    var baseComponent = TryFindBaseComponent(componentSymbol, context);
    var createdComponent = new ConcreteComponent(componentSymbol, componentsDepsByLevels, baseComponent);

    if (TryFindOverridenComponent(componentSymbol, context) is { } overridenComponent)
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
    GeneratorExecutionContext context)
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

        if (TryGetDependency(dependencySymbol, visited, context) is { } dependency)
        {
          dependencies.Add(new ComponentDependencyDescriptor(dependency, modifier));
        }

        alreadyAddedDependencySymbols.Add(dependencySymbol);
      }
    }

    return dependencies;
  }

  private IComponentDependency? TryGetDependency(
    INamedTypeSymbol dependencySymbol, ISet<INamedTypeSymbol> visited, GeneratorExecutionContext context)
  {
    if (dependencySymbol.TypeKind == TypeKind.Class)
    {
      var dependencyComponent = ToComponentInfo(dependencySymbol, visited, context);
      return new ConcreteComponentDependency(dependencyComponent);
    }

    if (dependencySymbol.TypeKind == TypeKind.Interface)
    {
      if (dependencySymbol.MetadataName == Constants.GenericIEnumerable)
      {
        return new CollectionDependency(dependencySymbol, this);
      }

      return new NonCollectionInterfaceDependency(dependencySymbol, this);
    }

    return null;
  }

  private void AddToInterfacesToImplementationsMap(IConcreteComponent concreteComponent)
  {
    if (concreteComponent.ComponentSymbol.TypeKind is not TypeKind.Class) return;

    var allInterfaces = ExtractInterfaces(concreteComponent.ComponentSymbol);
    if (allInterfaces.Count == 0) return;

    foreach (var @interface in allInterfaces)
    {
      myCache.AddInterfaceImplementation(@interface, concreteComponent);
    }
  }

  private IConcreteComponent? TryFindBaseComponent(INamedTypeSymbol symbol, GeneratorExecutionContext context)
  {
    if (symbol.BaseType is not { } baseType ||
        !mySymbolsCache.CheckIfManualfacComponent(baseType))
    {
      return null;
    }

    return ToComponentInfo(baseType, new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default), context);
  }

  private IConcreteComponent? TryFindOverridenComponent(INamedTypeSymbol symbol, GeneratorExecutionContext context)
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

    var visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
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
  
  private IEnumerable<INamedTypeSymbol> GetComponentTypesFrom(IModuleSymbol module)
  {
    return GetAllNamespacesFrom(module.GlobalNamespace)
      .SelectMany(ns => ns.GetTypeMembers())
      .Where(mySymbolsCache.CheckIfManualfacComponent);
  }

  private static IEnumerable<INamespaceSymbol> GetAllNamespacesFrom(INamespaceSymbol namespaceSymbol)
  {
    yield return namespaceSymbol;
    foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
    {
      foreach (var nextNamespace in GetAllNamespacesFrom(childNamespace))
      {
        yield return nextNamespace;
      }
    }
  }
}