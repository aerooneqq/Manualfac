using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Fields;

internal class GeneratedFieldModel : IGeneratedModel
{
  private readonly AccessModifier myAccessModifier;
  private readonly bool myReadonly;
  private readonly bool myIsStatic;
  private readonly string? myDefaultValue;


  public string TypeName { get; }
  public string Name { get; }


  public GeneratedFieldModel(
    string typeName,
    string fieldName,
    AccessModifier accessModifier = AccessModifier.Private,
    bool @readonly = true,
    bool isStatic = false,
    string? defaultValue = null)
  {
    TypeName = typeName;
    Name = fieldName;

    myAccessModifier = accessModifier;
    myReadonly = @readonly;
    myIsStatic = isStatic;
    myDefaultValue = defaultValue;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myAccessModifier.CreateModifierString()).AppendSpace();

    if (myIsStatic)
    {
      sb.Append("static").AppendSpace();
    }

    if (myReadonly)
    {
      sb.Append("readonly").AppendSpace();
    }

    sb.Append(TypeName).AppendSpace().Append(Name);

    if (myDefaultValue is { })
    {
      sb.AppendSpace().AppendEq().AppendSpace().Append(myDefaultValue);
    }

    sb.AppendSemicolon().AppendNewLine();
  }
}