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
          "Resolve", new[] { GenericType }, GenericType, GenerateResolveMethod,
          ImmutableList<GeneratedParameterModel>.Empty, AccessModifier.Internal)
      });
  }


  private void GenerateResolveMethod(StringBuilder sb, int indent)
  {
    foreach (var component in myStorage.AllComponents)
    {
      var condition = $"{GenericType} is {component.ComponentSymbol.GetFullName()}";
      StringBuilderCookies.If(sb, indent, condition, (builder, newIndent) =>
      {
        var resolveExpr = component.CreateContainerResolveExpression();
        builder.AppendIndent(newIndent).Append("return ").Append(resolveExpr).AppendSemicolon();
      });

      sb.AppendNewLine();
    }

    foreach (var pair in myStorage.InterfacesToComponents)
    {
      var impls = pair.Value;
      var interfaceName = pair.Key.GetFullName();
      if (impls.Count == 1)
      {
        var condition = $"{GenericType} is {interfaceName}";
        StringBuilderCookies.If(sb, indent, condition, (builder, newIndent) =>
        {
          var resolveExpression = impls[0].CreateContainerResolveExpression();
          builder.AppendIndent(newIndent).Append("return ").Append(resolveExpression).AppendSemicolon();
        });
      }
      else
      {
        var condition = $"{GenericType} is IEnumerable<{interfaceName}>";
        StringBuilderCookies.If(sb, indent, condition, (builder, newIndent) =>
        {
          var initializers = string.Join(",", impls.Select(impl => impl.CreateContainerResolveExpression()));
          var array = $"new {interfaceName}[] {{{initializers}}}";
          builder.AppendIndent(newIndent).Append("return ").Append(array).AppendSemicolon();
        });
      }

      sb.AppendNewLine();
    }
  }
  
  
  public void GenerateInto(StringBuilder sb, int indent) => myGeneratedClassModel.GenerateInto(sb, indent);
}