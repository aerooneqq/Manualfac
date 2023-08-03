using System.Collections.Immutable;
using System.Diagnostics;
using Manualfac.Exceptions.BeforeAfterRelation;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Caches;

internal class ComponentsCache(ManualfacSymbols manualfacSymbols)
{
  private readonly Dictionary<INamedTypeSymbol, IComponent> myCache = new(SymbolEqualityComparer.Default);
  private readonly List<IComponent> myAllComponents = new();

  private readonly Dictionary<INamedTypeSymbol, List<IComponent>> myInterfacesToComponents =
    new(SymbolEqualityComparer.Default);


  public IReadOnlyList<IComponent> AllComponents => myAllComponents;
  public IReadOnlyDictionary<INamedTypeSymbol, List<IComponent>> InterfacesToComponents => myInterfacesToComponents;


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

  public void AdjustInterfaceImplementations(Func<IComponent, IComponent> adjustComponent)
  {
    foreach (var @interface in myInterfacesToComponents.Keys)
    {
      var list = myInterfacesToComponents[@interface];
      myInterfacesToComponents[@interface] = list.Select(adjustComponent).Distinct().ToList();
    }
  }

  public void SortByBeforeAfterRelation()
  {
    foreach (var key in myInterfacesToComponents.Keys)
    {
      var parentToChildren = new Dictionary<IComponent, List<IComponent>>();
      var originalImpls = myInterfacesToComponents[key];

      foreach (var component in originalImpls)
      {
        var afterTypes = component.Symbol.GetAttributesTypeArguments(manualfacSymbols.AfterAttributeBase);
        var beforeTypes = component.Symbol.GetAttributesTypeArguments(manualfacSymbols.BeforeAttributeBase);

        foreach (var afterType in afterTypes)
        {
          var afterComponent = myCache[afterType];
          AssertBeforeAfterRelationInvariants(originalImpls, key, component, afterComponent);

          parentToChildren.AddToList(component, afterComponent);
        }

        foreach (var beforeType in beforeTypes)
        {
          var beforeComponent = myCache[beforeType];
          AssertBeforeAfterRelationInvariants(originalImpls, key, component, beforeComponent);

          parentToChildren.AddToList(myCache[beforeType], component);
        }
      }

      var sorted = ComponentsTopologicalSorter.Sort(
        originalImpls,
        component => parentToChildren.TryGetValue(component, out var children) switch
        {
          true => children,
          false => ImmutableArray<IComponent>.Empty
        });

      myInterfacesToComponents[key] = sorted;
    }
  }

  private static void AssertBeforeAfterRelationInvariants(
    ICollection<IComponent> originalImpls, INamedTypeSymbol @interface, IComponent component, IComponent reference)
  {
    if (!originalImpls.Contains(reference))
    {
      throw new ReferenceToOtherInterfaceComponentException(@interface, component.Symbol, reference.Symbol);
    }

    if (reference == component)
    {
      throw new SelfReferenceInBeforeAfterRelationException(component.Symbol);
    }
  }
}