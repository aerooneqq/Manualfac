using Manualfac.Generators.Util;

namespace Manualfac.Generators.Components.Dependencies;

internal class ComponentDependencyDescriptor
{
  public IComponentDependency Dependency { get; }
  public AccessModifier Modifier { get; }


  public ComponentDependencyDescriptor(IComponentDependency dependency, AccessModifier modifier)
  {
    Dependency = dependency;
    Modifier = modifier;
  }


  public void Deconstruct(out IComponentDependency dependency, out AccessModifier modifier)
  {
    dependency = Dependency;
    modifier = Modifier;
  }
}