using Microsoft.CodeAnalysis;

namespace Manualfac.Components.Dependencies;

internal interface IComponentDependency
{
  INamedTypeSymbol DependencyTypeSymbol { get; }

  IReadOnlyList<IComponent> ResolveUnderlyingConcreteComponents();
}