using System.Text;
using Manualfac.Generators.Util;

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
  private readonly IReadOnlyList<GeneratedConstructorModel> myConstructors;
  private readonly IReadOnlyList<GeneratedFieldModel> myFields;
  private readonly IReadOnlyList<GeneratedMethodModel> myMethods;
  private readonly GeneratedClassAccessModifier myModifier;


  public GeneratedClassModel(
    string name,
    IReadOnlyList<GeneratedConstructorModel> constructors,
    IReadOnlyList<GeneratedFieldModel> fields,
    IReadOnlyList<GeneratedMethodModel> methods,
    GeneratedClassAccessModifier modifier = GeneratedClassAccessModifier.Public)
  {
    myName = name;
    myConstructors = constructors;
    myFields = fields;
    myMethods = methods;
    myModifier = modifier;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myModifier.CreateModifierString())
      .Append(" partial class ").Append(myName).AppendNewLine();
    
    using (var cookie = StringBuilderCookies.CurlyBraces(sb, indent))
    {
      foreach (var field in myFields)
      {
        field.GenerateInto(sb, cookie.Indent);
      }
      
      if (myFields.Count > 0) sb.AppendNewLine();
      
      foreach (var constructor in myConstructors)
      {
        constructor.GenerateInto(sb, cookie.Indent);
      }
      
      if (myConstructors.Count > 0) sb.AppendNewLine();
      
      foreach (var method in myMethods)
      {
        method.GenerateInto(sb, cookie.Indent);
      }
    }
  }
}