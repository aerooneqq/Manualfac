using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ComponentInfo
{
  private IReadOnlyList<ComponentInfo>? myOrderedDependencies;

  public INamedTypeSymbol ComponentSymbol { get; }
  public HashSet<ComponentInfo> Dependencies { get; }


  public string ShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + ShortName : ShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;

  
  public ComponentInfo(INamedTypeSymbol componentSymbol, HashSet<ComponentInfo> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = dependencies;
  }


  public IReadOnlyList<ComponentInfo> GetOrCreateOrderedListOfDependencies()
  {
    if (myOrderedDependencies is null)
    {
      myOrderedDependencies = Dependencies.OrderBy(dep => dep.ShortName).ToList();
    }

    return myOrderedDependencies;
  }
}