using System.Collections.Immutable;
using Manualfac.Generators.Models;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Components;

internal static class ExtensionsForIComponentInfo
{
  public static string CreateContainerName(this IConcreteComponent concreteComponent) => $"{concreteComponent.TypeShortName}Container";

  public static string CreateContainerResolveExpression(this IConcreteComponent concreteComponent) =>
    $"{concreteComponent.CreateContainerName()}.{Constants.ResolveMethod}()";
  
  public static GeneratedUsingsModel ToDependenciesUsingsModel(this IConcreteComponent concreteComponent) => 
    new(concreteComponent.Dependencies
      .SelectMany(dep => dep.ResolveUnderlyingConcreteComponents().Select(c => c.Namespace))
      .Where(ns => ns is { })
      .Distinct()
      .ToList()!);
  
  public static GeneratedComponentFileModel ToGeneratedFileModel(this IConcreteComponent concreteComponent) => new(concreteComponent);

  public static GeneratedClassModel ToGeneratedClassModel(this IConcreteComponent concreteComponent)
  {
    var fields = concreteComponent.ExtractGeneratedFieldsModels();
    var className = concreteComponent.TypeShortName;

    return new GeneratedClassModel(
      className,
      new[] { new GeneratedConstructorModel(className, fields) },
      fields,
      ImmutableList<GeneratedMethodModel>.Empty);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFieldsModels(this IConcreteComponent concreteComponent) => 
    concreteComponent.OrderedDependencies
      .Select(dep => new GeneratedFieldModel(dep.Component.DependencyTypeSymbol.GetFullName(), $"my{dep.Component.DependencyTypeSymbol.Name}", dep.Modifier))
      .ToList();
}