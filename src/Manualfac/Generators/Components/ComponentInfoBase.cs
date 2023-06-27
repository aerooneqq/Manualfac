using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal abstract class ComponentInfoBase : IComponentInfo
{
  public abstract INamedTypeSymbol ComponentSymbol { get; }
  public abstract HashSet<IComponentDependency> Dependencies { get; }
  public abstract IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  public string TypeShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + TypeShortName : TypeShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;
  
  
  public IReadOnlyList<IComponentInfo> ResolveConcreteDependencies()
  {
    var concreteDependencies = new List<IComponentInfo>();
    foreach (var (component, _) in OrderedDependencies)
    {
      concreteDependencies.AddRange(component.ResolveUnderlyingConcreteComponents());
    }

    return concreteDependencies;
  }
}