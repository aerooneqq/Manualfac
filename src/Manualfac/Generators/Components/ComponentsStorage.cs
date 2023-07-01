using Manualfac.Exceptions;
using Manualfac.Generators.Components.Caches;
using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    var compilation = context.Compilation;

    var componentsDepsByLevels = ExtractComponentsDependencies(componentSymbol, visited, context);
    var baseComponent = TryFindBaseComponent(componentSymbol, context);
    var createdComponent = new ConcreteComponent(componentSymbol, componentsDepsByLevels, baseComponent);

    if (baseComponent is { })
    {
      myOverridesCache.AddOverride(createdComponent, baseComponent);
    }
    
    myCache.UpdateExistingComponent(componentSymbol, createdComponent);
    
    AddToInterfacesToImplementationsMap(createdComponent, compilation);
    
    return createdComponent;
  }

  private IReadOnlyList<IReadOnlyList<(IComponentDependency, AccessModifier)>> ExtractComponentsDependencies(
    INamedTypeSymbol componentSymbol, 
    ISet<INamedTypeSymbol> visited,
    GeneratorExecutionContext context)
  {
    var compilation = context.Compilation;
    var alreadyAddedDependencySymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    var componentsDepsByLevels = new List<IReadOnlyList<(IComponentDependency, AccessModifier)>>();

    foreach (var level in ExtractDependenciesByLevels(componentSymbol, compilation))
    {
      var currentLevel = new List<(IComponentDependency, AccessModifier)>();
      foreach (var (attributeSyntax, dependencyTypes) in level)
      {
        var modifier = ExtractAccessModifierOrDefault(attributeSyntax, compilation);

        foreach (var dependencySymbol in dependencyTypes.OfType<INamedTypeSymbol>())
        {
          if (alreadyAddedDependencySymbols.Contains(dependencySymbol))
          {
            throw new DuplicatedDependencyException(componentSymbol, dependencySymbol);
          }

          if (dependencySymbol.TypeKind == TypeKind.Class)
          {
            var dependencyComponent = ToComponentInfo(dependencySymbol, visited, context);
            currentLevel.Add((new ConcreteComponentDependency(dependencyComponent), modifier));
          }
          else if (dependencySymbol.TypeKind == TypeKind.Interface)
          {
            if (dependencySymbol.MetadataName == Constants.GenericIEnumerable)
            {
              currentLevel.Add((new CollectionDependency(dependencySymbol, this), modifier));
            }
            else
            {
              currentLevel.Add((new NonCollectionInterfaceDependency(dependencySymbol, this), modifier));
            }
          }

          alreadyAddedDependencySymbols.Add(dependencySymbol);
        }

        componentsDepsByLevels.Add(currentLevel);
      }
    }

    return componentsDepsByLevels;
  }

  private void AddToInterfacesToImplementationsMap(IConcreteComponent concreteComponent, Compilation compilation)
  {
    if (concreteComponent.ComponentSymbol.TypeKind is not TypeKind.Class) return;

    var allInterfaces = ExtractInterfaces(concreteComponent.ComponentSymbol, compilation);
    if (allInterfaces.Count == 0) return;

    foreach (var @interface in allInterfaces)
    {
      myCache.AddInterfaceImplementation(@interface, concreteComponent);
    }
  }

  private IConcreteComponent? TryFindBaseComponent(INamedTypeSymbol symbol, GeneratorExecutionContext context)
  {
    var compilation = context.Compilation;
    var overridesAttributes = ExtractAttributeByNameWithTypeArgs(symbol, Constants.OverridesAttribute, compilation);
    var baseSymbols = overridesAttributes.SelectMany(pair => pair.Interfaces).OfType<INamedTypeSymbol>().ToList();
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

  private IReadOnlyList<INamedTypeSymbol> ExtractInterfaces(INamedTypeSymbol symbol, Compilation compilation)
  {
    var asAttributes = ExtractAttributeByNameWithTypeArgs(symbol, Constants.AsAttribute, compilation).ToList();
    if (asAttributes.Count == 0)
    {
      return symbol.AllInterfaces;
    }

    return asAttributes.SelectMany(pair => pair.Interfaces).OfType<INamedTypeSymbol>().ToList();
  }

  private IReadOnlyList<IReadOnlyList<(AttributeSyntax, IEnumerable<ITypeSymbol?>)>> ExtractDependenciesByLevels(
    INamedTypeSymbol symbol, 
    Compilation compilation)
  {
    var dependenciesByLevels = new List<IReadOnlyList<(AttributeSyntax, IEnumerable<ITypeSymbol?>)>>();
    var immediateDependencies = ExtractAttributeByNameWithTypeArgs(symbol, Constants.DependsOnAttribute, compilation)
      .ToList();
    
    dependenciesByLevels.Add(immediateDependencies);
    var current = symbol.BaseType;
    
    while (current is { })
    {
      var nextLevelDeps = ExtractAttributeByNameWithTypeArgs(current, Constants.DependsOnAttribute, compilation)
        .ToList();
      
      dependenciesByLevels.Add(nextLevelDeps);
      current = current.BaseType;
    }

    return dependenciesByLevels;
  }

  private static IEnumerable<(AttributeSyntax Attribute, IEnumerable<ITypeSymbol?> Interfaces)> ExtractAttributeByNameWithTypeArgs(
    ISymbol symbol,
    string attributeName,
    Compilation compilation)
  {
    return symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.Name == attributeName)
      .Select(attr => attr.ApplicationSyntaxReference?.GetSyntax())
      .OfType<AttributeSyntax>()
      .Select(attributeSyntax =>
      {
        var typeArgs = attributeSyntax.DescendantNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault();
        return (Attribute: attributeSyntax, TypeArgs: typeArgs);
      })
      .Where(tuple => tuple.TypeArgs is { })
      .Select(tuple =>
      {
        var args = tuple.TypeArgs!.Arguments;
        var types = args.Select(arg => compilation.GetSemanticModel(arg.SyntaxTree).GetTypeInfo(arg).Type);
        return (tuple.Attribute, Types: types);
      });
  }

  private AccessModifier ExtractAccessModifierOrDefault(AttributeSyntax attributeSyntax, Compilation compilation)
  {
    if (attributeSyntax.ArgumentList is not { } argumentList) return AccessModifier.Private;
    
    foreach (var argument in argumentList.Arguments)
    {
      if (argument.Expression is not MemberAccessExpressionSyntax accessSyntax) continue;
      var foundSymbol = compilation.GetSemanticModel(accessSyntax.SyntaxTree).GetSymbolInfo(accessSyntax).Symbol;
      
      if (foundSymbol is IFieldSymbol { Type.Name: "AccessModifier" } fieldSymbol)
      {
        return foundSymbol.Name switch
        {
          nameof(AccessModifier.Internal) => AccessModifier.Internal,
          nameof(AccessModifier.Protected) => AccessModifier.Protected,
          nameof(AccessModifier.Private) => AccessModifier.Private,
          nameof(AccessModifier.Public) => AccessModifier.Public,
          nameof(AccessModifier.PrivateProtected) => AccessModifier.PrivateProtected,
          nameof(AccessModifier.ProtectedInternal) => AccessModifier.ProtectedInternal,
          _ => throw new ArgumentOutOfRangeException(fieldSymbol.Name)
        };
      }
    }

    return AccessModifier.Private;
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