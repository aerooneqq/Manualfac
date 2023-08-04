using Manualfac.Components.Dependencies;
using Manualfac.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Components;

internal class Component : IComponent
{
  public INamedTypeSymbol Symbol { get; }
  public IComponentDependencies Dependencies { get; }
  public IComponent? BaseComponent { get; }

  public string TypeShortName => Symbol.Name;
  public string FullName => Symbol.GetFullName();
  public string Namespace => Symbol.ContainingNamespace.Name;


  public Component(
    INamedTypeSymbol componentSymbol,
    IReadOnlyList<ComponentDependencyDescriptor> dependenciesByLevels,
    IComponent? baseComponent)
  {
    BaseComponent = baseComponent;
    Symbol = componentSymbol;
    Dependencies = new ComponentDependenciesImpl(this, dependenciesByLevels);
  }


  public IReadOnlyList<IComponent> ResolveConcreteDependencies()
  {
    var concreteDependencies = new List<IComponent>();
    foreach (var (component, _) in Dependencies.AllOrderedDependencies)
    {
      concreteDependencies.AddRange(component.ResolveUnderlyingConcreteComponents());
    }

    return concreteDependencies;
  }
}