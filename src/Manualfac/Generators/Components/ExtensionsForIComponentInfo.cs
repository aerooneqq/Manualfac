using System.Collections.Immutable;
using Manualfac.Generators.Models;

namespace Manualfac.Generators.Components;

internal static class ExtensionsForIComponentInfo
{
  public static string CreateContainerName(this IComponentInfo component) => $"{component.TypeShortName}Container";
  
  public static GeneratedUsingsModel ToDependenciesUsingsModel(this IComponentInfo component) => 
    new(component.Dependencies
      .SelectMany(dep => dep.ResolveUnderlyingConcreteComponents().Select(c => c.Namespace))
      .Where(ns => ns is { })
      .Distinct()
      .ToList()!);
  
  public static GeneratedComponentFileModel ToGeneratedFileModel(this IComponentInfo component) => new(component);

  public static GeneratedClassModel ToGeneratedClassModel(this IComponentInfo component)
  {
    var fields = component.ExtractGeneratedFieldsModels();
    var className = component.TypeShortName;

    return new GeneratedClassModel(
      className,
      new[] { new GeneratedConstructorModel(className, fields) },
      fields,
      ImmutableList<GeneratedMethodModel>.Empty);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFieldsModels(this IComponentInfo component) => 
    component.OrderedDependencies
      .Select(dep => new GeneratedFieldModel(dep.Component.DependencyTypeSymbol.Name, $"my{dep.Component.DependencyTypeSymbol.Name}", dep.Modifier))
      .ToList();
}