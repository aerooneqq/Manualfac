namespace Manualfac.Generators.Components.Dependencies;

internal class ComponentDependenciesImpl : IComponentDependencies
{
  private readonly IConcreteComponent myComponent;
  

  public HashSet<IComponentDependency> AllDependenciesSet { get; }
  public IReadOnlyList<ComponentDependencyDescriptor> ImmediateDependencies { get; }


  public ComponentDependenciesImpl(
    IConcreteComponent thisComponent, 
    IReadOnlyList<ComponentDependencyDescriptor> immediateDependencies)
  {
    myComponent = thisComponent;
    ImmediateDependencies = immediateDependencies;
    var allDependencies = DependenciesByLevels.SelectMany(deps => deps.Select(dep => dep.Dependency));
    AllDependenciesSet = new HashSet<IComponentDependency>(allDependencies);
  }
  
  
  public IEnumerable<IReadOnlyList<ComponentDependencyDescriptor>> DependenciesByLevels
  {
    get
    {
      yield return ImmediateDependencies;
      var current = myComponent.BaseComponent;
      while (current is { })
      {
        yield return current.Dependencies.ImmediateDependencies;
        current = current.BaseComponent;
      }
    }
  }
  
  public IEnumerable<ComponentDependencyDescriptor> AllOrderedDependencies
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
}