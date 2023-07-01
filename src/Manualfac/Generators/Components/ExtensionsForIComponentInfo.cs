using System.Collections.Immutable;
using System.Data.SqlTypes;
using Manualfac.Generators.Components.Dependencies;
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

  public static GeneratedClassModel ToGeneratedClassModel(this IConcreteComponent component)
  {
    GeneratedBaseConstructorModel? baseConstructorModel = null;
    if (component.BaseComponent is { })
    {
      var paramNames = new List<string>();
      var index = component.Dependencies.ImmediateDependencies.Count;
      foreach (var baseClassesDependencies in component.Dependencies.DependenciesByLevels.Skip(1))
      {
        foreach (var _ in baseClassesDependencies)
        {
          paramNames.Add(GeneratedConstructorUtil.GetComponentParamName(index++));
        }
      }
      
      baseConstructorModel = new GeneratedBaseConstructorModel(paramNames);
    }

    var constructorParams = component.ExtractFieldsForConstructor();
    var fields = component.ExtractFields();
    var className = component.TypeShortName;
    var constructorModel = new GeneratedConstructorModel(className, constructorParams, fields, baseConstructorModel);
    
    return new GeneratedClassModel(
      className,
      new[] { constructorModel },
      component.ExtractFields(),
      ImmutableList<GeneratedMethodModel>.Empty);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractFields(
    this IConcreteComponent component) =>
    ExtractGeneratedFieldModelsInternal(component.Dependencies.ImmediateDependencies);
  
  private static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFieldModelsInternal(
    IEnumerable<ComponentDependencyDescriptor> descriptors)
  {
    return descriptors.Select(ToGeneratedFieldModel).ToList();
  }

  private static GeneratedFieldModel ToGeneratedFieldModel(ComponentDependencyDescriptor descriptor)
  {
    var symbol = descriptor.Dependency.DependencyTypeSymbol; 
    return new GeneratedFieldModel(symbol.GetFullName(), $"my{symbol.Name}", descriptor.Modifier);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractFieldsForConstructor(
    this IConcreteComponent component) =>
    ExtractGeneratedFieldModelsInternal(component.Dependencies.AllOrderedDependencies);
}