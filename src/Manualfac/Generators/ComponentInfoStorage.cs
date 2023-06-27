using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Generators;

internal class ComponentInfoStorage
{
  private readonly HashSet<INamedTypeSymbol> myComponentsSymbols;
  private readonly HashSet<INamedTypeSymbol> myNotComponentsSymbols;
  private readonly Dictionary<INamedTypeSymbol, ComponentInfo> myCache;
  private readonly List<ComponentInfo> myAllComponents;
  private readonly Dictionary<INamedTypeSymbol, List<ComponentInfo>> myInterfacesToComponents;
  private readonly List<ComponentInfo> myComponentsWithoutInterfaces;


  public IReadOnlyList<ComponentInfo> AllComponents => myAllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<ComponentInfo>> InterfacesToComponents => myInterfacesToComponents;
  public IReadOnlyList<ComponentInfo> ComponentsWithoutInterfaces => myComponentsWithoutInterfaces;


  public ComponentInfoStorage()
  {
    myCache = new Dictionary<INamedTypeSymbol, ComponentInfo>(SymbolEqualityComparer.Default);
    myComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myNotComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myAllComponents = new List<ComponentInfo>();
    myInterfacesToComponents = new Dictionary<INamedTypeSymbol, List<ComponentInfo>>(SymbolEqualityComparer.Default);
    myComponentsWithoutInterfaces = new List<ComponentInfo>();
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
  
  private ComponentInfo ToComponentInfo(
    INamedTypeSymbol symbol, ISet<INamedTypeSymbol> visited, GeneratorExecutionContext context)
  {
    if (myCache.TryGetValue(symbol, out var existingComponent)) return existingComponent;
    if (visited.Contains(symbol)) throw new CyclicDependencyException();
    if (!CheckIfManualfacComponent(symbol)) throw new TypeSymbolIsNotManualfacComponentException(symbol);

    visited.Add(symbol);
    var compilation = context.Compilation;
    var dependencies = new List<(ComponentInfo, AccessModifier)>();
    
    foreach (var (attributeSyntax, types) in ExtractDependencies(symbol, compilation))
    {
      var modifier = ExtractAccessModifierOrDefault(attributeSyntax, compilation);
      
      foreach (var typeSymbol in types.OfType<INamedTypeSymbol>())
      {
        var dependencyComponent = ToComponentInfo(typeSymbol, visited, context);
        dependencies.Add((dependencyComponent, modifier));
      }
    }
    
    var createdComponent = new ComponentInfo(symbol, dependencies);
    myCache[symbol] = createdComponent;
    
    AddToInterfacesToImplementationsMap(createdComponent);
    
    return createdComponent;
  }

  private void AddToInterfacesToImplementationsMap(ComponentInfo componentInfo)
  {
    var allInterfaces = componentInfo.ComponentSymbol.AllInterfaces;
    if (allInterfaces.Length == 0)
    {
      myComponentsWithoutInterfaces.Add(componentInfo);
      return;
    }
    
    foreach (var @interface in allInterfaces)
    {
      if (myInterfacesToComponents.TryGetValue(@interface, out var components))
      {
        components.Add(componentInfo);
      }
      else
      {
        myInterfacesToComponents[@interface] = new List<ComponentInfo> { componentInfo };
      }
    }
  }

  private static IEnumerable<(AttributeSyntax, IEnumerable<ITypeSymbol?>)> ExtractDependencies(
    INamedTypeSymbol symbol, Compilation compilation)
  {
    return symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.Name == "DependsOnAttribute")
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