using System.Collections.Immutable;
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
  public static string CreateContainerName(this ComponentInfo component) => $"{component.TypeShortName}Container";
  
  public static GeneratedUsingsModel ToDependenciesUsingsModel(this ComponentInfo component) => 
    new(component.Dependencies.Select(dep => dep.Namespace).Where(ns => ns is { }).Distinct().ToList()!);
  
  public static GeneratedComponentFileModel ToGeneratedFileModel(this ComponentInfo component) => new(component);

  public static GeneratedClassModel ToGeneratedClassModel(this ComponentInfo component)
  {
    var fields = component.ExtractGeneratedFieldsModels();
    var className = component.TypeShortName;

    return new GeneratedClassModel(
      className,
      new[] { new GeneratedConstructorModel(className, fields) },
      fields,
      ImmutableList<GeneratedMethodModel>.Empty);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFieldsModels(this ComponentInfo component) => 
    component.OrderedDependencies
      .Select(dep => new GeneratedFieldModel(dep.Component.TypeShortName, $"my{dep.Component.TypeShortName}", dep.Modifier))
      .ToList();
}