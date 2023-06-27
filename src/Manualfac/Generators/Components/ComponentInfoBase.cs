using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal abstract class ComponentInfoBase
{
  public abstract INamedTypeSymbol ComponentSymbol { get; }
  public abstract HashSet<IComponentInfo> Dependencies { get; }
  public abstract IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  public string TypeShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + TypeShortName : TypeShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;
}