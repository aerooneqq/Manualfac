namespace Manualfac.Components.Dependencies;

internal interface IComponentDependencies
{
  public IEnumerable<ComponentDependencyDescriptor> AllOrderedDependencies { get; }
  public IEnumerable<IReadOnlyList<ComponentDependencyDescriptor>> DependenciesByLevels { get; }
  public HashSet<IComponentDependency> AllDependenciesSet { get; }
  public IReadOnlyList<ComponentDependencyDescriptor> ImmediateDependencies { get; }
}