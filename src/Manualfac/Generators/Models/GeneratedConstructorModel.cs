using System.Text;
using Manualfac.Generators.Models.Fields;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedConstructorModel : IGeneratedModel
{
  private readonly string myContainingTypeName;
  private readonly IReadOnlyList<GeneratedFieldModel> myConstructorParameters;
  private readonly IReadOnlyList<GeneratedFieldModel> myFieldsToInitialize;
  private readonly GeneratedBaseConstructorModel? myBaseConstructorModel;


  public GeneratedConstructorModel(
    string containingTypeName,
    IReadOnlyList<GeneratedFieldModel> constructorParameters,
    IReadOnlyList<GeneratedFieldModel> fieldsToInitialize,
    GeneratedBaseConstructorModel? baseConstructorModel = null)
  {
    myContainingTypeName = containingTypeName;
    myConstructorParameters = constructorParameters;
    myFieldsToInitialize = fieldsToInitialize;
    myBaseConstructorModel = baseConstructorModel;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("public ").Append(myContainingTypeName);

    using (var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      var index = 0;
      foreach (var field in myConstructorParameters)
      {
        sb.AppendIndent(cookie.Indent).Append(field.TypeName).AppendSpace()
          .Append(GeneratedConstructorUtil.GetComponentParamName(index++))
          .AppendComma().AppendNewLine();
      }

      if (index > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }

    myBaseConstructorModel?.GenerateInto(sb, indent);

    using (var cookie = StringBuilderCookies.CurlyBraces(sb.AppendNewLine(), indent))
    {
      var index = 0;
      foreach (var field in myFieldsToInitialize)
      {
        sb.AppendIndent(cookie.Indent).Append(field.Name)
          .AppendSpace().AppendEq().AppendSpace().Append(GeneratedConstructorUtil.GetComponentParamName(index++))
          .AppendSemicolon().AppendNewLine();
      }

      //remove last new line
      sb.Remove(sb.Length - 1, 1);
    }
  }
}

internal static class GeneratedConstructorUtil
{
  public static string GetComponentParamName(int index) => $"c{index}";
}