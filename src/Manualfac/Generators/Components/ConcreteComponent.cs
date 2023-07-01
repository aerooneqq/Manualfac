using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class ConcreteComponent : IConcreteComponent
{
  public INamedTypeSymbol ComponentSymbol { get; }
  public IComponentDependencies Dependencies { get; }
  public IConcreteComponent? BaseComponent { get; }
  
  public string TypeShortName => ComponentSymbol.Name;
  public string FullName => ComponentSymbol.GetFullName();
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;


  public ConcreteComponent(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> dependenciesByLevels,
    IConcreteComponent? baseComponent)
  {
    BaseComponent = baseComponent;
    ComponentSymbol = componentSymbol;
    Dependencies = new ComponentDependenciesImpl(this, dependenciesByLevels);
  }
  
  
  public IReadOnlyList<IConcreteComponent> ResolveConcreteDependencies()
  {
    var concreteDependencies = new List<IConcreteComponent>();
    foreach (var (component, _) in Dependencies.AllOrderedDependencies)
    {
      concreteDependencies.AddRange(component.ResolveUnderlyingConcreteComponents());
    }

    return concreteDependencies;
  }
}