using System.Text;

namespace Manualfac.Generators.Models;

internal class GeneratedFieldModel
{
  private readonly AccessModifier myAccessModifier;
  private readonly bool myReadonly;


  public string TypeName { get; }
  public string Name { get; }


  public GeneratedFieldModel(
    string typeName,
    string name,
    AccessModifier accessModifier = AccessModifier.Private,
    bool @readonly = true)
  {
    TypeName = typeName;
    Name = name;
    myAccessModifier = accessModifier;
    myReadonly = @readonly;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myAccessModifier.CreateModifierString()).AppendSpace();

    if (myReadonly)
    {
      sb.Append("readonly").AppendSpace();
    }
    
    sb.Append(TypeName).AppendSpace().Append(Name).AppendSemicolon().AppendNewLine();
  }
}