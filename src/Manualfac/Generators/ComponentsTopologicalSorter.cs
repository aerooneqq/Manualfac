using Manualfac.Exceptions;

namespace Manualfac.Generators;

internal static class ComponentsTopologicalSorter
{
  private enum ComponentState
  {
    NotVisited,
    Black,
    Gray
  }
  
  public static IReadOnlyList<IComponentInfo> Sort(IReadOnlyList<IComponentInfo> components)
  {
    var visited = components.ToDictionary(static c => c, static _ => ComponentState.NotVisited);
    var result = new List<IComponentInfo>();
    
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
    IComponentInfo current, Dictionary<IComponentInfo, ComponentState> visited, List<IComponentInfo> result)
  {
    visited[current] = ComponentState.Gray;
    foreach (var dependency in current.Dependencies)
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