using Manualfac.Generators.Models;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ComponentInfo
{
  public INamedTypeSymbol ComponentSymbol { get; }
  public HashSet<ComponentInfo> Dependencies { get; }
  public IReadOnlyList<(ComponentInfo Component, AccessModifier Modifier)> OrderedDependencies { get; }


  public string TypeShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + TypeShortName : TypeShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;

  
  public ComponentInfo(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyCollection<(ComponentInfo Component, AccessModifier Modifier)> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = new HashSet<ComponentInfo>(dependencies.Select(dep => dep.Component));
    OrderedDependencies = dependencies.OrderBy(dep => dep.Component.TypeShortName).ToList();
  }
}

internal static class ComponentInfoExtensions
{
  public static GeneratedClassModel ToGeneratedClassModel(this ComponentInfo component)
  {
    return new GeneratedClassModel(component.TypeShortName, component.ExtractGeneratedFields());
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFields(this ComponentInfo component)
  {
    return component.OrderedDependencies
      .Select(dep => new GeneratedFieldModel(dep.Component.TypeShortName, $"my{dep.Component.TypeShortName}", dep.Modifier))
      .ToList();
  }
}