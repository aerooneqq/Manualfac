using Manualfac.Components.Dependencies;
using Manualfac.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Components;

internal class Component : IComponent
{
  public INamedTypeSymbol Symbol { get; }
  public IComponentDependencies Dependencies { get; }
  public IComponent? BaseComponent { get; }
  public IReadOnlyList<INamedTypeSymbol> RawDependencies { get; }

  public string TypeShortName => Symbol.Name;
  public string FullName => Symbol.GetFullName();
  public string Namespace => Symbol.GetFullNamespaceName();
  public bool ManualInitialization { get; }


  public Component(
    INamedTypeSymbol componentSymbol,
    IReadOnlyList<ComponentDependencyDescriptor> dependenciesByLevels,
    IComponent? baseComponent,
    bool manualInitialization)
  {
    BaseComponent = baseComponent;
    Symbol = componentSymbol;
    Dependencies = new ComponentDependenciesImpl(this, dependenciesByLevels);
    ManualInitialization = manualInitialization;
    RawDependencies = dependenciesByLevels.Select(d => d.Dependency.DependencyTypeSymbol).ToList();
  }


  public IReadOnlyList<IComponent> ResolveConcreteDependencies()
  {
    if (ManualInitialization)
    {
      return [];
    }

    var concreteDependencies = new List<IComponent>();
    foreach (var (component, _) in Dependencies.AllOrderedDependencies)
    {
      concreteDependencies.AddRange(component.ResolveUnderlyingConcreteComponents());
    }

    return concreteDependencies;
  }
}