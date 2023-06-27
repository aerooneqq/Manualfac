using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal interface IComponentInfo
{
  INamedTypeSymbol ComponentSymbol { get; }
  HashSet<IComponentInfo> Dependencies { get; }
  IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }
}