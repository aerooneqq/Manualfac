using Manualfac.Generators.Components.Dependencies;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal interface IConcreteComponent
{
  INamedTypeSymbol ComponentSymbol { get; }
  IComponentDependencies Dependencies { get; }
  IConcreteComponent? BaseComponent { get; }
  
  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }

  IReadOnlyList<IConcreteComponent> ResolveConcreteDependencies();
}

internal interface IComponentDependencies
{
  public IEnumerable<(IComponentDependency Component, AccessModifier Modifier)> AllOrderedDependencies { get; }
  public IReadOnlyList<IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)>> DependenciesByLevels { get; }
  public HashSet<IComponentDependency> AllDependenciesSet { get; }
  public IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> ImmediateDependencies { get; }
}

internal class ComponentDependenciesImpl : IComponentDependencies
{
  public IReadOnlyList<IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)>> DependenciesByLevels { get; }
  public HashSet<IComponentDependency> AllDependenciesSet { get; }

  public IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> ImmediateDependencies =>
    DependenciesByLevels.First();

  public IEnumerable<(IComponentDependency Component, AccessModifier Modifier)> AllOrderedDependencies
  {
    get
    {
      foreach (var dependenciesByLevel in DependenciesByLevels)
      {
        foreach (var pair in dependenciesByLevel)
        {
          yield return pair;
        }
      }
    }
  }

  
  public ComponentDependenciesImpl(
    IReadOnlyList<IReadOnlyList<(IComponentDependency, AccessModifier)>> dependenciesByLevels)
  {
    DependenciesByLevels = dependenciesByLevels;
    var allComponents = DependenciesByLevels.SelectMany(deps => deps.Select(dep => dep.Component));
    AllDependenciesSet = new HashSet<IComponentDependency>(allComponents);
  }
}