using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class ComponentInfo : ComponentInfoBase, IComponentInfo
{
  public override INamedTypeSymbol ComponentSymbol { get; }
  public override HashSet<IComponentDependency> Dependencies { get; }
  public override IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  
  public ComponentInfo(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyCollection<(IComponentDependency Component, AccessModifier Modifier)> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = new HashSet<IComponentDependency>(dependencies.Select(dep => dep.Component));
    OrderedDependencies = dependencies.OrderBy(dep => dep.Component.DependencyTypeSymbol.Name).ToList();
  }
}