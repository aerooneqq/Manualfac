using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Caches;

internal class ComponentsCache
{
  private readonly Dictionary<INamedTypeSymbol, IComponent> myCache;
  private readonly List<IComponent> myAllComponents;
  private readonly Dictionary<INamedTypeSymbol, List<IComponent>> myInterfacesToComponents;

  
  public IReadOnlyList<IComponent> AllComponents => myAllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IComponent>> InterfacesToComponents => myInterfacesToComponents;
  

  public ComponentsCache()
  {
    myCache = new Dictionary<INamedTypeSymbol, IComponent>(SymbolEqualityComparer.Default);
    myAllComponents = new List<IComponent>();
    myInterfacesToComponents = new Dictionary<INamedTypeSymbol, List<IComponent>>(SymbolEqualityComparer.Default);
  }


  public IComponent? TryGetExistingComponent(INamedTypeSymbol symbol)
  {
    return myCache.TryGetValue(symbol, out var existingComponent) ? existingComponent : null;
  }

  public void UpdateExistingComponent(INamedTypeSymbol symbol, IComponent component)
  {
    myCache[symbol] = component;
    myAllComponents.Add(component);
  }

  public void AddInterfaceImplementation(INamedTypeSymbol @interface, IComponent component)
  {
    Debug.Assert(@interface.TypeKind == TypeKind.Interface);
    if (myInterfacesToComponents.TryGetValue(@interface, out var components))
    {
      components.Add(component);
    }
    else
    {
      myInterfacesToComponents[@interface] = new List<IComponent> { component };
    }
  }
}