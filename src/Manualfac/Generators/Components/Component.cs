using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class Component : IComponent
{
  public INamedTypeSymbol ComponentSymbol { get; }
  public IComponentDependencies Dependencies { get; }
  public IComponent? BaseComponent { get; }

  public string TypeShortName => ComponentSymbol.Name;
  public string FullName => ComponentSymbol.GetFullName();
  public string Namespace => ComponentSymbol.ContainingNamespace.Name;


  public Component(
    INamedTypeSymbol componentSymbol,
    IReadOnlyList<ComponentDependencyDescriptor> dependenciesByLevels,
    IComponent? baseComponent)
  {
    BaseComponent = baseComponent;
    ComponentSymbol = componentSymbol;
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