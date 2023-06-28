using System.Diagnostics;
using Manualfac.Exceptions;
using Manualfac.Generators.Components.Dependencies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Generators.Components;

internal class ComponentsStorage
{
  private readonly HashSet<INamedTypeSymbol> myComponentsSymbols;
  private readonly HashSet<INamedTypeSymbol> myNotComponentsSymbols;
  private readonly Dictionary<INamedTypeSymbol, IConcreteComponent> myCache;
  private readonly List<IConcreteComponent> myAllComponents;
  private readonly Dictionary<INamedTypeSymbol, List<IConcreteComponent>> myInterfacesToComponents;
  private readonly List<IConcreteComponent> myComponentsWithoutInterfaces;


  public IReadOnlyList<IConcreteComponent> AllComponents => myAllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IConcreteComponent>> InterfacesToComponents => myInterfacesToComponents;
  public IReadOnlyList<IConcreteComponent> ComponentsWithoutInterfaces => myComponentsWithoutInterfaces;


  public ComponentsStorage()
  {
    myCache = new Dictionary<INamedTypeSymbol, IConcreteComponent>(SymbolEqualityComparer.Default);
    myComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myNotComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myAllComponents = new List<IConcreteComponent>();
    myInterfacesToComponents = new Dictionary<INamedTypeSymbol, List<IConcreteComponent>>(SymbolEqualityComparer.Default);
    myComponentsWithoutInterfaces = new List<IConcreteComponent>();
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

      var componentTypes = GetComponentTypesFrom(module);
      var componentInfos = componentTypes
        .Select(type => ToComponentInfo(type, new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default), context));
      
      myAllComponents.AddRange(componentInfos);

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
    INamedTypeSymbol symbol, ISet<INamedTypeSymbol> visited, GeneratorExecutionContext context)
  {
    if (myCache.TryGetValue(symbol, out var existingComponent)) return existingComponent;
    if (visited.Contains(symbol)) throw new CyclicDependencyException();
    if (!CheckIfManualfacComponent(symbol)) throw new TypeSymbolIsNotManualfacComponentException(symbol);

    visited.Add(symbol);
    var compilation = context.Compilation;
    var dependencies = new List<(IComponentDependency, AccessModifier)>();
    
    foreach (var (attributeSyntax, types) in ExtractDependencies(symbol, compilation))
    {
      var modifier = ExtractAccessModifierOrDefault(attributeSyntax, compilation);
      
      foreach (var typeSymbol in types.OfType<INamedTypeSymbol>())
      {
        if (typeSymbol.TypeKind == TypeKind.Class)
        {
          var dependencyComponent = ToComponentInfo(typeSymbol, visited, context);
          dependencies.Add((new ConcreteComponentDependency(dependencyComponent), modifier));
        }
        else if (typeSymbol.TypeKind == TypeKind.Interface)
        {
          dependencies.Add((new NonCollectionInterfaceDependency(typeSymbol, this), modifier));
        }
      }
    }
    
    var createdComponent = new ConcreteComponent(symbol, dependencies);
    myCache[symbol] = createdComponent;
    
    AddToInterfacesToImplementationsMap(createdComponent, compilation);
    
    return createdComponent;
  }

  private void AddToInterfacesToImplementationsMap(IConcreteComponent concreteComponent, Compilation compilation)
  {
    if (concreteComponent.ComponentSymbol.TypeKind is not TypeKind.Class) return;

    var allInterfaces = ExtractInterfaces(concreteComponent.ComponentSymbol, compilation);
    if (allInterfaces.Count == 0)
    {
      myComponentsWithoutInterfaces.Add(concreteComponent);
      return;
    }
    
    foreach (var @interface in allInterfaces)
    {
      Debug.Assert(@interface.TypeKind == TypeKind.Interface);
      if (myInterfacesToComponents.TryGetValue(@interface, out var components))
      {
        components.Add(concreteComponent);
      }
      else
      {
        myInterfacesToComponents[@interface] = new List<IConcreteComponent> { concreteComponent };
      }
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

  private static IEnumerable<(AttributeSyntax, IEnumerable<ITypeSymbol?>)> ExtractDependencies(
    INamedTypeSymbol symbol, Compilation compilation)
  {
    return ExtractAttributeByNameWithTypeArgs(symbol, "DependsOnAttribute", compilation);
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

  private bool CheckIfManualfacComponent(INamedTypeSymbol symbol)
  {
    if (myNotComponentsSymbols.Contains(symbol)) return false;
    if (myComponentsSymbols.Contains(symbol)) return true;

    if (symbol.GetAttributes().Any(IsManualfacAttribute))
    {
      myComponentsSymbols.Add(symbol);
      return true;
    }

    myNotComponentsSymbols.Add(symbol);
    return false;
  }

  private IEnumerable<INamedTypeSymbol> GetComponentTypesFrom(IModuleSymbol module)
  {
    return GetAllNamespacesFrom(module.GlobalNamespace)
      .SelectMany(ns => ns.GetTypeMembers())
      .Where(CheckIfManualfacComponent);
  }
  
  private static bool IsManualfacAttribute(AttributeData attribute)
  {
    var attributeClass = attribute.AttributeClass;
    while (attributeClass is { })
    {
      if (attributeClass.Name == "ManualfacAttribute") return true;
      attributeClass = attributeClass.BaseType;
    }

    return false;
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