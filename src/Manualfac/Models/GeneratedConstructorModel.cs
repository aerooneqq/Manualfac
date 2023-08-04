using System.Text;
using Manualfac.Models.Fields;
using Manualfac.Util;

namespace Manualfac.Models;

internal class GeneratedConstructorModel(
  string containingTypeName,
  IReadOnlyList<GeneratedFieldModel> constructorParameters,
  IReadOnlyList<GeneratedFieldModel> fieldsToInitialize,
  GeneratedBaseConstructorModel? baseConstructorModel = null
) : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("public ").Append(containingTypeName);

    using (var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      var index = 0;
      foreach (var field in constructorParameters)
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

    baseConstructorModel?.GenerateInto(sb, indent);

    using (var cookie = StringBuilderCookies.CurlyBraces(sb.AppendNewLine(), indent))
    {
      var index = 0;
      foreach (var field in fieldsToInitialize)
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