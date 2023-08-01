using Manualfac.Exceptions;
using Manualfac.Generators.Components;

namespace Manualfac.Generators.Util;

internal static class ComponentsTopologicalSorter
{
  private enum ComponentState
  {
    NotVisited,
    Black,
    Gray
  }
  
  public static List<IComponent> Sort(
    IReadOnlyList<IComponent> components, 
    Func<IComponent, IEnumerable<IComponent>> childrenProvider)
  {
    var visited = components.ToDictionary(static c => c, static _ => ComponentState.NotVisited);
    var result = new List<IComponent>();
    
    foreach (var component in components)
    {
      if (visited[component] == ComponentState.NotVisited)
      {
        if (Dfs(component, visited, result, childrenProvider))
        {
          throw new CyclicDependencyException();
        }
      }
    }

    return result;
  }

  private static bool Dfs(
    IComponent current, 
    Dictionary<IComponent, ComponentState> visited, 
    List<IComponent> result,
    Func<IComponent, IEnumerable<IComponent>> childrenProvider)
  {
    visited[current] = ComponentState.Gray;
    foreach (var dependency in childrenProvider(current))
    {
      if (visited[dependency] == ComponentState.Gray)
      {
        return true;
      }

      if (visited[dependency] == ComponentState.NotVisited)
      {
        if (Dfs(dependency, visited, result, childrenProvider)) return true;
      }
    }

    visited[current] = ComponentState.Black;
    
    result.Add(current);
    return false;
  }
}