﻿using System.Collections.Immutable;
using Manualfac.Components.Dependencies;
using Manualfac.Models;
using Manualfac.Models.Fields;
using Manualfac.Models.Methods;
using Manualfac.Util;
using Manualfac.Util.Naming;

namespace Manualfac.Components;

internal static class ExtensionsForIComponentInfo
{
  public static string CreateContainerName(this IComponent component) =>
    $"{component.TypeShortName}Container";

  public static string CreateContainerFullName(this IComponent component) =>
    $"{component.Namespace}{(string.IsNullOrWhiteSpace(component.Namespace) ? "" : ".")}{component.CreateContainerName()}";

  public static string CreateContainerResolveExpression(this IComponent component) =>
    $"{component.CreateContainerFullName()}.{Constants.ResolveMethod}()";

  public static GeneratedUsingsModel ToDependenciesUsingsModel(this IComponent component) =>
    new(component.RawDependencies
      .Select(dep => dep.GetFullNamespaceName())
      .Where(ns => ns is { } && !string.IsNullOrWhiteSpace(ns))
      .Distinct()
      .ToList());

  public static GeneratedComponentFileModel ToGeneratedFileModel(this IComponent component, NamingStyle style) =>
    new(component, style);

  public static GeneratedClassModel ToGeneratedClassModel(this IComponent component, NamingStyle namingStyle)
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

    var constructorParams = component.ExtractFieldsForConstructor(namingStyle);
    var fields = component.ExtractFields(namingStyle);
    var className = component.TypeShortName;
    var constructorModel = new GeneratedConstructorModel(className, constructorParams, fields, baseConstructorModel);

    return new GeneratedClassModel(
      className,
      new[] { constructorModel },
      component.ExtractFields(namingStyle),
      ImmutableList<GeneratedMethodModel>.Empty);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractFields(this IComponent component, NamingStyle namingStyle) =>
    ExtractGeneratedFieldModelsInternal(component.Dependencies.ImmediateDependencies, namingStyle);

  private static IReadOnlyList<GeneratedFieldModel> ExtractGeneratedFieldModelsInternal(
    IEnumerable<ComponentDependencyDescriptor> descriptors, NamingStyle namingStyle)
  {
    return descriptors.Select(desc => ToGeneratedFieldModel(desc, namingStyle)).ToList();
  }

  private static GeneratedFieldModel ToGeneratedFieldModel(
    ComponentDependencyDescriptor descriptor, NamingStyle namingStyle)
  {
    var symbol = descriptor.Dependency.DependencyTypeSymbol;
    var fullName = symbol.GetFullName();
    var name = namingStyle.ApplyNamingStyleTo(symbol.GetAssociatedFieldNameBase());

    return new GeneratedFieldModel(fullName, name, descriptor.Modifier);
  }

  public static IReadOnlyList<GeneratedFieldModel> ExtractFieldsForConstructor(
    this IComponent component, NamingStyle namingStyle) =>
    ExtractGeneratedFieldModelsInternal(component.Dependencies.AllOrderedDependencies, namingStyle);
}