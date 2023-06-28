using Manualfac.Generators.Components.Dependencies;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal interface IConcreteComponent
{
  INamedTypeSymbol ComponentSymbol { get; }
  HashSet<IComponentDependency> Dependencies { get; }
  IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }

  IReadOnlyList<IConcreteComponent> ResolveConcreteDependencies();
}