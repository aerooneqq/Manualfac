namespace Manualfac.Generators.Components.Dependencies;

internal class ComponentDependenciesImpl : IComponentDependencies
{
  private readonly IConcreteComponent myComponent;
  

  public HashSet<IComponentDependency> AllDependenciesSet { get; }
  public IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> ImmediateDependencies { get; }


  public ComponentDependenciesImpl(
    IConcreteComponent thisComponent, 
    IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> immediateDependencies)
  {
    myComponent = thisComponent;
    ImmediateDependencies = immediateDependencies;
    var allComponents = DependenciesByLevels.SelectMany(deps => deps.Select(dep => dep.Component));
    AllDependenciesSet = new HashSet<IComponentDependency>(allComponents);
  }
  
  
  public IEnumerable<IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)>> DependenciesByLevels
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
}