using System.Text;

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


  public unsafe void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("public ").Append(myContainingTypeName);

    using (StringBuilderCookies.DefaultBraces(sb, &indent, appendEndIndent: true))
    {
      var index = 0;
      foreach (var field in myFieldsToInitialize)
      {
        sb.AppendIndent(indent).Append(field.TypeName).AppendSpace().Append(GetComponentParamName(index++))
          .AppendComma().AppendNewLine();
      }
      
      if (index > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }
    
    using (StringBuilderCookies.CurlyBraces(sb.AppendNewLine(), &indent))
    {
      var index = 0;
      foreach (var field in myFieldsToInitialize)
      {
        sb.AppendIndent(indent).Append(field.Name)
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