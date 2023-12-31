﻿using System.Collections.Immutable;
using System.Text;
using Manualfac.Components;
using Manualfac.Models.Fields;
using Manualfac.Models.Methods;
using Manualfac.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Models.TopLevel;

internal class GeneratedContainerResolverModel : IGeneratedModel
{
  private const string GenericType = "TComponent";

  private readonly ComponentsStorage myStorage;
  private readonly GeneratedClassModel myGeneratedClassModel;


  public GeneratedContainerResolverModel(ComponentsStorage storage, IAssemblySymbol assemblySymbol)
  {
    myStorage = storage;
    var typeParam = new[] { GenericType };

    myGeneratedClassModel = new GeneratedClassModel(
      $"{assemblySymbol.Name}Resolver",
      ImmutableList<GeneratedConstructorModel>.Empty,
      ImmutableList<GeneratedFieldModel>.Empty,
      new[]
      {
        MethodFactory.InternalStaticGeneric("ResolveOrThrow", GenericType, typeParam, GenerateResolveMethod),
        MethodFactory.InternalStaticGeneric(
          "ResolveComponentsOrThrow", $"IEnumerable<{GenericType}>", typeParam, GenerateResolveComponentsMethods),
      });
  }


  private void GenerateResolveMethod(StringBuilder sb, int indent)
  {
    foreach (var component in myStorage.AllComponents)
    {
      var condition = CreateCondition(component.Symbol);
      StringBuilderCookies.If(sb, indent, condition, (builder, newIndent) =>
      {
        var resolveExpr = CreateResolveExpr(component);
        builder.AppendIndent(newIndent).Append("return ").Append(resolveExpr).AppendSemicolon();
      });

      sb.AppendNewLine();
    }

    foreach (var pair in myStorage.InterfacesToComponents)
    {
      var impls = pair.Value;
      if (impls.Count == 1)
      {
        var condition = CreateCondition(pair.Key);
        StringBuilderCookies.If(sb, indent, condition, (builder, newIndent) =>
        {
          var resolveExpression = CreateResolveExpr(impls[0]);
          builder.AppendIndent(newIndent).Append("return ").Append(resolveExpression).AppendSemicolon();
        });

        sb.AppendNewLine();
      }
    }

    sb.AppendIndent(indent).Append("throw new ArgumentOutOfRangeException();");
  }

  private static string CreateCondition(INamedTypeSymbol symbol) =>
    $"typeof({GenericType}) == typeof({symbol.GetFullName()})";

  private static string CreateResolveExpr(IComponent component) =>
    $"({GenericType})((object){component.CreateContainerResolveExpression()})";

  private void GenerateResolveComponentsMethods(StringBuilder sb, int indent)
  {
    foreach (var pair in myStorage.InterfacesToComponents)
    {
      var impls = pair.Value;
      if (impls.Count > 1)
      {
        var condition = CreateCondition(pair.Key);
        StringBuilderCookies.If(sb, indent, condition, (builder, newIndent) =>
        {
          var initializers = string.Join(",", impls.Select(CreateResolveExpr));
          var array = $"new {GenericType}[] {{{initializers}}}";
          builder.AppendIndent(newIndent).Append("return ").Append(array).AppendSemicolon();
        });

        sb.AppendNewLine();
      }
    }

    sb.AppendIndent(indent).Append("throw new ArgumentOutOfRangeException();");
  }


  public void GenerateInto(StringBuilder sb, int indent) => myGeneratedClassModel.GenerateInto(sb, indent);
}