using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Fields;

internal class GeneratedFieldModel(
    string typeName,
    string fieldName,
    AccessModifier accessModifier = AccessModifier.Private,
    bool @readonly = true,
    bool isStatic = false,
    string? defaultValue = null
) : IGeneratedModel
{
  public string TypeName { get; } = typeName;
  public string Name { get; } = fieldName;


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(accessModifier.CreateModifierString()).AppendSpace();

    if (isStatic)
    {
      sb.Append("static").AppendSpace();
    }

    if (@readonly)
    {
      sb.Append("readonly").AppendSpace();
    }

    sb.Append(TypeName).AppendSpace().Append(Name);

    if (defaultValue is { })
    {
      sb.AppendSpace().AppendEq().AppendSpace().Append(defaultValue);
    }

    sb.AppendSemicolon().AppendNewLine();
  }
}