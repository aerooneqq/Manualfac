using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Caches;

internal class ComponentsCache
{
  private readonly Dictionary<INamedTypeSymbol, IConcreteComponent> myCache;
  private readonly List<IConcreteComponent> myAllComponents;
  private readonly Dictionary<INamedTypeSymbol, List<IConcreteComponent>> myInterfacesToComponents;

  
  public IReadOnlyList<IConcreteComponent> AllComponents => myAllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IConcreteComponent>> InterfacesToComponents => myInterfacesToComponents;
  

  public ComponentsCache()
  {
    myCache = new Dictionary<INamedTypeSymbol, IConcreteComponent>(SymbolEqualityComparer.Default);
    myAllComponents = new List<IConcreteComponent>();
    myInterfacesToComponents = new Dictionary<INamedTypeSymbol, List<IConcreteComponent>>(SymbolEqualityComparer.Default);
  }


  public IConcreteComponent? TryGetExistingComponent(INamedTypeSymbol symbol)
  {
    return myCache.TryGetValue(symbol, out var existingComponent) ? existingComponent : null;
  }

  public void UpdateExistingComponent(INamedTypeSymbol symbol, IConcreteComponent component)
  {
    myCache[symbol] = component;
    myAllComponents.Add(component);
  }

  public void AddInterfaceImplementation(INamedTypeSymbol @interface, IConcreteComponent component)
  {
    Debug.Assert(@interface.TypeKind == TypeKind.Interface);
    if (myInterfacesToComponents.TryGetValue(@interface, out var components))
    {
      components.Add(component);
    }
    else
    {
      myInterfacesToComponents[@interface] = new List<IConcreteComponent> { component };
    }
  }
}