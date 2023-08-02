using Manualfac.Generators.Components.Dependencies;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal interface IComponent
{
  INamedTypeSymbol Symbol { get; }
  IComponentDependencies Dependencies { get; }
  IComponent? BaseComponent { get; }

  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }

  IReadOnlyList<IComponent> ResolveConcreteDependencies();
}