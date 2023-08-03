using Manualfac.Generators.Util;

namespace Manualfac.Generators.Components.Dependencies;

internal class ComponentDependencyDescriptor(IComponentDependency dependency, AccessModifier modifier)
{
  public IComponentDependency Dependency { get; } = dependency;
  public AccessModifier Modifier { get; } = modifier;


  public void Deconstruct(out IComponentDependency dependency, out AccessModifier modifier)
  {
    dependency = Dependency;
    modifier = Modifier;
  }
}