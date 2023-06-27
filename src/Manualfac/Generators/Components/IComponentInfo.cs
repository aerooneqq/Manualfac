using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal interface IComponentInfo
{
  INamedTypeSymbol ComponentSymbol { get; }
  HashSet<IComponentInfo> Dependencies { get; }
  IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }

  IReadOnlyList<ComponentInfo> ResolveConcreteDependencies();
  IReadOnlyList<ComponentInfo> ResolveUnderlyingConcreteComponents();
}