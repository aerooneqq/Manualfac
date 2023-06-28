using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class ConcreteComponentDependency : IComponentDependency
{
  private readonly IConcreteComponent myConcreteComponent;

  
  public INamedTypeSymbol DependencyTypeSymbol => myConcreteComponent.ComponentSymbol;

  
  public ConcreteComponentDependency(IConcreteComponent concreteComponent)
  {
    myConcreteComponent = concreteComponent;
  }
  
  
  public IReadOnlyList<IConcreteComponent> ResolveUnderlyingConcreteComponents() => new[] { myConcreteComponent };
}