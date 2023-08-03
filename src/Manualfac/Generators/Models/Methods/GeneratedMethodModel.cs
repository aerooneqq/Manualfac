using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Methods;

internal class GeneratedMethodModel(
    string name,
    IReadOnlyList<string> typeParameters,
    string returnTypeName,
    Action<StringBuilder, int> bodyGenerator,
    IReadOnlyList<GeneratedParameterModel> parameters,
    AccessModifier modifier = AccessModifier.Public,
    bool isStatic = false,
    bool isPartial = false
) : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(modifier.CreateModifierString()).AppendSpace();

    if (isStatic) sb.Append("static").AppendSpace();
    if (isPartial) sb.Append("partial").AppendSpace();

    sb.Append(returnTypeName).AppendSpace().Append(name);

    if (typeParameters is { Count: > 0 })
    {
      sb.Append('<');
      foreach (var genericParameter in typeParameters)
      {
        sb.Append(genericParameter).AppendComma();
      }

      if (typeParameters.Count > 0)
      {
        //remove last comma
        sb.Remove(sb.Length - 1, 1);
      }

      sb.Append('>');
    }

    using (var bracesCookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      foreach (var parameterModel in parameters)
      {
        parameterModel.GenerateInto(sb, bracesCookie.Indent);
        sb.AppendComma().AppendSpace();
      }

      if (parameters.Count > 0)
      {
        //remove last space and comma
        sb.Remove(sb.Length - 2, 2);
      }
    }

    sb.AppendNewLine();
    using var cookie = StringBuilderCookies.CurlyBraces(sb, indent);
    bodyGenerator(sb, cookie.Indent);
  }
}