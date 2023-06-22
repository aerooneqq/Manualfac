using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Generators;

internal class ComponentInfoStorage
{
  private readonly HashSet<INamedTypeSymbol> myComponentsSymbols;
  private readonly HashSet<INamedTypeSymbol> myNotComponentsSymbols;
  private readonly Dictionary<INamedTypeSymbol, ComponentInfo> myCache;
  private readonly List<ComponentInfo> myComponents;


  public IReadOnlyList<ComponentInfo> Components => myComponents; 


  public ComponentInfoStorage()
  {
    myCache = new Dictionary<INamedTypeSymbol, ComponentInfo>(SymbolEqualityComparer.Default);
    myComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myNotComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myComponents = new List<ComponentInfo>();
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
      
      myComponents.AddRange(GetComponentTypesFrom(module).Select(type => ToComponentInfo(type, context)));

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
  
  private ComponentInfo ToComponentInfo(INamedTypeSymbol symbol, GeneratorExecutionContext context)
  {
    var dependencies = new HashSet<ComponentInfo>();
    var compilation = context.Compilation;

    var dependenciesSymbols = symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.Name == "DependsOnAttribute")
      .Select(attr => attr.ApplicationSyntaxReference?.GetSyntax())
      .OfType<AttributeSyntax>()
      .Select(attributeSyntax => attributeSyntax.DescendantNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault())
      .Where(typeArgs => typeArgs is { })
      .SelectMany(typeArgsSyntax => typeArgsSyntax!.Arguments)
      .Select(genericArg => compilation.GetSemanticModel(genericArg.SyntaxTree).GetTypeInfo(genericArg).Type)
      .OfType<INamedTypeSymbol>();

    foreach (var typeSymbol in dependenciesSymbols)
    {
      if (myCache.TryGetValue(typeSymbol, out var existingComponent))
      {
        dependencies.Add(existingComponent);
        continue;
      }

      var dependencyComponent = ToComponentInfo(typeSymbol, context);
      myCache[typeSymbol] = dependencyComponent;
      dependencies.Add(dependencyComponent);
    }
    
    return new ComponentInfo(symbol, dependencies);
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