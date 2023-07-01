using Manualfac.Generators.Components.Dependencies;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal interface IConcreteComponent
{
  INamedTypeSymbol ComponentSymbol { get; }
  IComponentDependencies Dependencies { get; }
  IConcreteComponent? BaseComponent { get; }
  
  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }

  IReadOnlyList<IConcreteComponent> ResolveConcreteDependencies();
}