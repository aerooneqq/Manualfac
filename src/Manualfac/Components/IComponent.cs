using Manualfac.Components.Dependencies;
using Microsoft.CodeAnalysis;

namespace Manualfac.Components;

internal interface IComponent
{
  INamedTypeSymbol Symbol { get; }
  IComponentDependencies Dependencies { get; }
  IComponent? BaseComponent { get; }

  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }
  bool ManualInitialization { get; }

  IReadOnlyList<IComponent> ResolveConcreteDependencies();
}