using Manualfac.Exceptions;
using Manualfac.Generators.Components.Caches;
using Manualfac.Generators.Components.Dependencies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Generators.Components;

internal class ComponentsStorage
{
  private readonly ComponentAndNonComponentSymbols mySymbolsCache;
  private readonly ComponentsCache myCache;


  public IReadOnlyList<IConcreteComponent> AllComponents => myCache.AllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IConcreteComponent>> InterfacesToComponents => myCache.InterfacesToComponents;


  public ComponentsStorage()
  {
    mySymbolsCache = new ComponentAndNonComponentSymbols();
    myCache = new ComponentsCache();
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
    if (!mySymbolsCache.CheckIfManualfacComponent(componentSymbol)) throw new TypeSymbolIsNotManualfacComponentException(componentSymbol);

    visited.Add(componentSymbol);
    var compilation = context.Compilation;
    var dependencies = new List<(IComponentDependency, AccessModifier)>();
    var alreadyAddedDependencySymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    
    foreach (var (attributeSyntax, dependencyTypes) in ExtractDependencies(componentSymbol, compilation))
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
          dependencies.Add((new ConcreteComponentDependency(dependencyComponent), modifier));
        }
        else if (dependencySymbol.TypeKind == TypeKind.Interface)
        {
          if (dependencySymbol.MetadataName == "IEnumerable`1")
          {
            dependencies.Add((new CollectionDependency(dependencySymbol, this), modifier));
          }
          else
          {
            dependencies.Add((new NonCollectionInterfaceDependency(dependencySymbol, this), modifier));
          }
        }

        alreadyAddedDependencySymbols.Add(dependencySymbol);
      }
    }
    
    var createdComponent = new ConcreteComponent(componentSymbol, dependencies);
    myCache.UpdateExistingComponent(componentSymbol, createdComponent);
    
    AddToInterfacesToImplementationsMap(createdComponent, compilation);
    
    return createdComponent;
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

  private IReadOnlyList<INamedTypeSymbol> ExtractInterfaces(INamedTypeSymbol symbol, Compilation compilation)
  {
    var asAttributes = ExtractAttributeByNameWithTypeArgs(symbol, "AsAttribute", compilation).ToList();
    if (asAttributes.Count == 0)
    {
      return symbol.AllInterfaces;
    }

    return asAttributes.SelectMany(pair => pair.Interfaces).OfType<INamedTypeSymbol>().ToList();
  }

  private IEnumerable<(AttributeSyntax, IEnumerable<ITypeSymbol?>)> ExtractDependencies(
    INamedTypeSymbol symbol, Compilation compilation)
  {
    const string DependsOnAttribute = "DependsOnAttribute";
    var immediateDependencies = ExtractAttributeByNameWithTypeArgs(symbol, DependsOnAttribute, compilation).ToList();
    var current = symbol.BaseType;
    
    while (current is { })
    {
      immediateDependencies.AddRange(ExtractAttributeByNameWithTypeArgs(current, DependsOnAttribute, compilation));
      current = current.BaseType;
    }

    return immediateDependencies;
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