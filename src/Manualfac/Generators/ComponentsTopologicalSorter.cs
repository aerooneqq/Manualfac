using Manualfac.Exceptions;
using Manualfac.Generators.Components;

namespace Manualfac.Generators;

internal static class ComponentsTopologicalSorter
{
  private enum ComponentState
  {
    NotVisited,
    Black,
    Gray
  }
  
  public static IReadOnlyList<IConcreteComponent> Sort(IReadOnlyList<IConcreteComponent> components)
  {
    var visited = components.ToDictionary(static c => c, static _ => ComponentState.NotVisited);
    var result = new List<IConcreteComponent>();
    
    foreach (var component in components)
    {
      if (visited[component] == ComponentState.NotVisited)
      {
        if (Dfs(component, visited, result))
        {
          throw new CyclicDependencyException();
        }
      }
    }

    result.Reverse();
    return result;
  }

  private static bool Dfs(
    IConcreteComponent current, Dictionary<IConcreteComponent, ComponentState> visited, List<IConcreteComponent> result)
  {
    visited[current] = ComponentState.Gray;
    foreach (var dependency in current.ResolveConcreteDependencies())
    {
      if (visited[dependency] == ComponentState.Gray)
      {
        return true;
      }

      if (visited[dependency] == ComponentState.NotVisited)
      {
        if (Dfs(dependency, visited, result)) return true;
      }
    }

    visited[current] = ComponentState.Black;
    
    result.Add(current);
    return false;
  }
}