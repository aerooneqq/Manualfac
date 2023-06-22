using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ComponentInfo
{
  private IReadOnlyList<INamedTypeSymbol>? myOrderedDependencies;

  public INamedTypeSymbol Component { get; }
  public HashSet<INamedTypeSymbol> Dependencies { get; }


  public string ShortName => Component.Name;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => Component.ContainingNamespace.Name;

  
  public ComponentInfo(INamedTypeSymbol component, HashSet<INamedTypeSymbol> dependencies)
  {
    Component = component;
    Dependencies = dependencies;
  }


  public IReadOnlyList<INamedTypeSymbol> GetOrCreateOrderedListOfDependencies()
  {
    if (myOrderedDependencies is null)
    {
      myOrderedDependencies = Dependencies.OrderBy(dep => dep.Name).ToList();
    }

    return myOrderedDependencies;
  }
}