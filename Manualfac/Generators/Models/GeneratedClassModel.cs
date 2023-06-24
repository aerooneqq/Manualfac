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
      
      if (myFields.Count > 0) sb.AppendNewLine();
      
      foreach (var constructor in myConstructors)
      {
        constructor.GenerateInto(sb, indent);
      }
      
      if (myConstructors.Count > 0) sb.AppendNewLine();
      
      foreach (var method in myMethods)
      {
        method.GenerateInto(sb, indent);
      }
    }
  }
}

internal class GeneratedMethodModel
{
  private readonly string myName;
  private readonly string myReturnTypeName;
  private readonly Action<StringBuilder, int> myBodyGenerator;
  private readonly AccessModifier myModifier;
  private readonly bool myIsStatic;
  private readonly bool myIsPartial;

  
  public GeneratedMethodModel(
    string name,
    string returnTypeName,
    Action<StringBuilder, int> bodyGenerator,
    AccessModifier modifier = AccessModifier.Public,
    bool isStatic = false,
    bool isPartial = false)
  {
    myName = name;
    myReturnTypeName = returnTypeName;
    myBodyGenerator = bodyGenerator;
    myModifier = modifier;
    myIsStatic = isStatic;
    myIsPartial = isPartial;
  }


  public unsafe void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myModifier.CreateModifierString()).AppendSpace();
    
    if (myIsStatic) sb.Append("static").AppendSpace();
    if (myIsPartial) sb.Append("partial").AppendSpace();

    sb.Append(myReturnTypeName).AppendSpace().Append(myName).Append("()").AppendNewLine();
    using (StringBuilderCookies.CurlyBraces(sb, &indent))
    {
      myBodyGenerator(sb, indent);
    }
  }
}