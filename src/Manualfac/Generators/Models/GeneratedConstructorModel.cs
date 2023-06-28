using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedConstructorModel
{
  private readonly string myContainingTypeName;
  private readonly IReadOnlyList<GeneratedFieldModel> myFieldsToInitialize;


  public GeneratedConstructorModel(string containingTypeName, IReadOnlyList<GeneratedFieldModel> fieldsToInitialize)
  {
    myContainingTypeName = containingTypeName;
    myFieldsToInitialize = fieldsToInitialize;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("public ").Append(myContainingTypeName);

    using (var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      var index = 0;
      foreach (var field in myFieldsToInitialize)
      {
        sb.AppendIndent(cookie.Indent).Append(field.TypeName).AppendSpace().Append(GetComponentParamName(index++))
          .AppendComma().AppendNewLine();
      }
      
      if (index > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }
    
    using (var cookie = StringBuilderCookies.CurlyBraces(sb.AppendNewLine(), indent))
    {
      var index = 0;
      foreach (var field in myFieldsToInitialize)
      {
        sb.AppendIndent(cookie.Indent).Append(field.Name)
          .AppendSpace().AppendEq().AppendSpace().Append(GetComponentParamName(index++))
          .AppendSemicolon().AppendNewLine();
      }

      //remove last new line
      sb.Remove(sb.Length - 1, 1);
    }

    return;

    static string GetComponentParamName(int index) => $"c{index}";
  }
}