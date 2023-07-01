using System.Collections.Immutable;
using System.Data.SqlTypes;
using Manualfac.Generators.Models;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Components;

internal static class ExtensionsForIComponentInfo
{
  public static string CreateContainerName(this IConcreteComponent concreteComponent) => 
    $"{concreteComponent.TypeShortName}Container";

  public static string CreateContainerResolveExpression(this IConcreteComponent concreteComponent) =>
    $"{concreteComponent.CreateContainerName()}.{Constants.ResolveMethod}()";
  
  public static GeneratedUsingsModel ToDependenciesUsingsModel(this IConcreteComponent concreteComponent) => 
    new(concreteComponent.Dependencies.AllDependenciesSet
      .SelectMany(dep => dep.ResolveUnderlyingConcreteComponents().Select(c => c.Namespace))
      .Where(ns => ns is { })
      .Distinct()
      .ToList()!);
  
  public static GeneratedComponentFileModel ToGeneratedFileModel(this IConcreteComponent concreteComponent) => 
    new(concreteComponent);

  public static GeneratedClassModel ToGeneratedClassModel(this IConcreteComponent concreteComponent)
  {
    var fields = concreteComponent.ExtractGeneratedFieldsModels();
    var className = concreteComponent.TypeShortName;

    GeneratedBaseConstructorModel? baseConstructorModel = null;
    if (concreteComponent.BaseComponent is { })
    {
      var paramNames = new List<string>();
      var index = concreteComponent.Dependencies.ImmediateDependencies.Count;
      foreach (var baseClassesDependencies in concreteComponent.Dependencies.DependenciesByLevels.Skip(1))
      {
        foreach (var _ in baseClassesDependencies)
        {
          paramNames.Add(GeneratedConstructorUtil.GetComponentParamName(index++));
        }
      }
      
      baseConstructorModel = new GeneratedBaseConstructorModel(paramNames);
    }

    return new GeneratedClassModel(
      className,
      new[] { new GeneratedConstructorModel(className, fields, baseConstructorModel) },
      fields,
      ImmutableList<GeneratedMethodModel>.Empty);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFieldsModels(this IConcreteComponent concreteComponent) => 
    concreteComponent.Dependencies.AllOrderedDependencies
      .Select(dep => new GeneratedFieldModel(dep.Component.DependencyTypeSymbol.GetFullName(), $"my{dep.Component.DependencyTypeSymbol.Name}", dep.Modifier))
      .ToList();
}