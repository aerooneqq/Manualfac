using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal interface IComponentDependency
{
  INamedTypeSymbol DependencyTypeSymbol { get; }
  
  IReadOnlyList<IComponentInfo> ResolveUnderlyingConcreteComponents();
}