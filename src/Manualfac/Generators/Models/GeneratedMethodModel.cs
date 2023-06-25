using System.Text;

namespace Manualfac.Generators.Models;

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