using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ComponentInfo
{
  public INamedTypeSymbol ComponentSymbol { get; }
  public HashSet<ComponentInfo> Dependencies { get; }
  public IReadOnlyList<(ComponentInfo Component, AccessModifier Modifier)> OrderedDependencies { get; }


  public string ShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + ShortName : ShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;

  
  public ComponentInfo(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyCollection<(ComponentInfo Component, AccessModifier Modifier)> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = new HashSet<ComponentInfo>(dependencies.Select(dep => dep.Component));
    OrderedDependencies = dependencies.OrderBy(dep => dep.Component.ShortName).ToList();
  }
}