using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Models.TopLevel;

internal class GeneratedContainerResolverModel : IGeneratedModel
{
  private const string GenericType = "TComponent";
  
  private readonly ComponentsStorage myStorage;
  private readonly GeneratedClassModel myGeneratedClassModel;


  public GeneratedContainerResolverModel(ComponentsStorage storage, IAssemblySymbol assemblySymbol)
  {
    myStorage = storage;
    myGeneratedClassModel = new GeneratedClassModel(
      $"{assemblySymbol.Name}Resolver",
      ImmutableList<GeneratedConstructorModel>.Empty,
      ImmutableList<GeneratedFieldModel>.Empty,
      new[]
      {
        new GeneratedMethodModel(
          "ResolveOrThrow", new[] { GenericType }, GenericType, GenerateResolveMethod,
          ImmutableList<GeneratedParameterModel>.Empty, AccessModifier.Internal, isStatic: true),
        
        new GeneratedMethodModel(
          "ResolveComponentsOrThrow", new[] { GenericType }, $"IEnumerable<{GenericType}>", GenerateResolveComponentsMethods,
          ImmutableList<GeneratedParameterModel>.Empty, AccessModifier.Internal, isStatic: true)
      });
  }


  private void GenerateResolveMethod(StringBuilder sb, int indent)
  {
    foreach (var component in myStorage.AllComponents)
    {
      var condition = CreateCondition(component.ComponentSymbol);
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