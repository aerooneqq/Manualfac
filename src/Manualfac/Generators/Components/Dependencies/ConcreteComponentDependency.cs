using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class ConcreteComponentDependency : IComponentDependency
{
  private readonly IComponent myComponent;

  
  public INamedTypeSymbol DependencyTypeSymbol => myComponent.ComponentSymbol;

  
  public ConcreteComponentDependency(IComponent component)
  {
    myComponent = component;
  }
  
  
  public IReadOnlyList<IComponent> ResolveUnderlyingConcreteComponents() => new[] { myComponent };
}