using System.Text;
using Manualfac.Models.Fields;
using Manualfac.Models.Methods;
using Manualfac.Util;

namespace Manualfac.Models;

internal class GeneratedClassModel(
  string name,
  IReadOnlyList<GeneratedConstructorModel> constructors,
  IReadOnlyList<GeneratedFieldModel> fields,
  IReadOnlyList<GeneratedMethodModel> methods,
  GeneratedClassAccessModifier modifier = GeneratedClassAccessModifier.Public,
  string? baseClass = null
) : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(modifier.CreateModifierString())
      .Append(" partial class ").Append(name);

    if (baseClass is { })
    {
      sb.Append(" : ").Append(baseClass);
    }

    sb.AppendNewLine();
    using var cookie = StringBuilderCookies.CurlyBraces(sb, indent);
    foreach (var field in fields)
    {
      field.GenerateInto(sb, cookie.Indent);
    }

    if (fields.Count > 0) sb.AppendNewLine();

    foreach (var constructor in constructors)
    {
      constructor.GenerateInto(sb, cookie.Indent);
    }

    if (constructors.Count > 0) sb.AppendNewLine();

    foreach (var method in methods)
    {
      method.GenerateInto(sb, cookie.Indent);
      sb.AppendNewLine().AppendNewLine();
    }
  }
}