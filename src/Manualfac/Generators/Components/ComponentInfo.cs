using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class ComponentInfo : IComponentInfo
{
  public INamedTypeSymbol ComponentSymbol { get; }
  public HashSet<IComponentDependency> Dependencies { get; }
  public IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  public string TypeShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + TypeShortName : TypeShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;


  public ComponentInfo(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyCollection<(IComponentDependency Component, AccessModifier Modifier)> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = new HashSet<IComponentDependency>(dependencies.Select(dep => dep.Component));
    OrderedDependencies = dependencies.OrderBy(dep => dep.Component.DependencyTypeSymbol.Name).ToList();
  }
  
  
  public IReadOnlyList<IComponentInfo> ResolveConcreteDependencies()
  {
    var concreteDependencies = new List<IComponentInfo>();
    foreach (var (component, _) in OrderedDependencies)
    {
      concreteDependencies.AddRange(component.ResolveUnderlyingConcreteComponents());
    }

    return concreteDependencies;
  }
}