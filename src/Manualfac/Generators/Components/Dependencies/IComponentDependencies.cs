namespace Manualfac.Generators.Components.Dependencies;

internal interface IComponentDependencies
{
  public IEnumerable<(IComponentDependency Component, AccessModifier Modifier)> AllOrderedDependencies { get; }
  public IEnumerable<IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)>> DependenciesByLevels { get; }
  public HashSet<IComponentDependency> AllDependenciesSet { get; }
  public IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> ImmediateDependencies { get; }
}