using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class ConcreteComponentDependency(IComponent component) : IComponentDependency
{
  public INamedTypeSymbol DependencyTypeSymbol => component.Symbol;


  public IReadOnlyList<IComponent> ResolveUnderlyingConcreteComponents() => new[] { component };
}