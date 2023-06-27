using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ComponentInfo : ComponentInfoBase, IComponentInfo
{
  public override INamedTypeSymbol ComponentSymbol { get; }
  public override HashSet<IComponentInfo> Dependencies { get; }
  public override IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  
  public ComponentInfo(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyCollection<(IComponentInfo Component, AccessModifier Modifier)> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = new HashSet<IComponentInfo>(dependencies.Select(dep => dep.Component));
    OrderedDependencies = dependencies.OrderBy(dep => dep.Component.TypeShortName).ToList();
  }
}