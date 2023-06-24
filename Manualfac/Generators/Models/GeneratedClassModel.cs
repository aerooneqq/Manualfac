using System.Text;

namespace Manualfac.Generators.Models;

internal enum GeneratedClassAccessModifier
{
  Public,
  Internal,
  File
}

internal static class GeneratedClassAccessModifierExtensions
{
  public static string CreateModifierString(this GeneratedClassAccessModifier modifier) => modifier switch
  {
    GeneratedClassAccessModifier.Public => "public",
    GeneratedClassAccessModifier.Internal => "internal",
    GeneratedClassAccessModifier.File => "file",
    _ => throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null)
  };
}

internal class GeneratedClassModel
{
  private readonly string myName;
  private readonly IReadOnlyList<GeneratedFieldModel> myFields;
  private readonly GeneratedClassAccessModifier myModifier;
  private readonly GeneratedConstructorModel myConstructorModel;


  public GeneratedClassModel(
    string name,
    IReadOnlyList<GeneratedFieldModel> fields,
    GeneratedClassAccessModifier modifier = GeneratedClassAccessModifier.Public)
  {
    myName = name;
    myFields = fields;
    myModifier = modifier;
    myConstructorModel = new GeneratedConstructorModel(name, fields);
  }


  public unsafe void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myModifier.CreateModifierString())
      .Append(" partial class ").Append(myName).AppendNewLine();
    
    using (StringBuilderCookies.CurlyBraces(sb, &indent))
    {
      foreach (var field in myFields)
      {
        field.GenerateInto(sb, indent);
      }
      
      sb.AppendNewLine();
      myConstructorModel.GenerateInto(sb, indent);
    }
  }
}