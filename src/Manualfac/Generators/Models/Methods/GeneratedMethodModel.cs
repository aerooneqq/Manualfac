using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Methods;

internal class GeneratedMethodModel : IGeneratedModel
{
  private readonly string myName;
  private readonly string myReturnTypeName;
  private readonly Action<StringBuilder, int> myBodyGenerator;
  private readonly IReadOnlyList<GeneratedParameterModel> myParameters;
  private readonly IReadOnlyList<string> myGenericParameters;
  private readonly AccessModifier myModifier;
  private readonly bool myIsStatic;
  private readonly bool myIsPartial;


  public GeneratedMethodModel(
    string name,
    IReadOnlyList<string> genericParameters,
    string returnTypeName,
    Action<StringBuilder, int> bodyGenerator,
    IReadOnlyList<GeneratedParameterModel> parameters,
    AccessModifier modifier = AccessModifier.Public,
    bool isStatic = false,
    bool isPartial = false)
  {
    myName = name;
    myReturnTypeName = returnTypeName;
    myBodyGenerator = bodyGenerator;
    myParameters = parameters;
    myGenericParameters = genericParameters;
    myModifier = modifier;
    myIsStatic = isStatic;
    myIsPartial = isPartial;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myModifier.CreateModifierString()).AppendSpace();

    if (myIsStatic) sb.Append("static").AppendSpace();
    if (myIsPartial) sb.Append("partial").AppendSpace();

    sb.Append(myReturnTypeName).AppendSpace().Append(myName);

    if (myGenericParameters is { Count: > 0 })
    {
      sb.Append('<');
      foreach (var genericParameter in myGenericParameters)
      {
        sb.Append(genericParameter).AppendComma();
      }

      if (myGenericParameters.Count > 0)
      {
        //remove last comma
        sb.Remove(sb.Length - 1, 1);
      }

      sb.Append('>');
    }

    using (var bracesCookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      foreach (var parameterModel in myParameters)
      {
        parameterModel.GenerateInto(sb, bracesCookie.Indent);
        sb.AppendComma().AppendSpace();
      }

      if (myParameters.Count > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }

    sb.AppendNewLine();
    using var cookie = StringBuilderCookies.CurlyBraces(sb, indent);
    myBodyGenerator(sb, cookie.Indent);
  }
}