using Microsoft.CodeAnalysis;

namespace Manualfac.Components.Dependencies;

internal class ConcreteComponentDependency(IComponent component) : IComponentDependency
{
  public INamedTypeSymbol DependencyTypeSymbol => component.Symbol;


  public IReadOnlyList<IComponent> ResolveUnderlyingConcreteComponents() => new[] { component };
}