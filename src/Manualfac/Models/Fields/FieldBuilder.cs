using Manualfac.Util;

namespace Manualfac.Models.Fields;

internal class FieldBuilder(string typeName, string fieldName)
{
  private bool myIsStatic;
  private bool myReadonly;
  private string? myInitialization;
  private AccessModifier myAccessModifier = AccessModifier.Private;


  public FieldBuilder Static(bool isStatic)
  {
    myIsStatic = isStatic;

    return this;
  }

  public FieldBuilder Readonly(bool @readonly)
  {
    myReadonly = @readonly;

    return this;
  }

  public FieldBuilder Private()
  {
    myAccessModifier = AccessModifier.Private;

    return this;
  }

  public FieldBuilder Initialization(string initialization)
  {
    myInitialization = initialization;

    return this;
  }

  public GeneratedFieldModel Build()
  {
    return new GeneratedFieldModel(typeName, fieldName, myAccessModifier, myReadonly, myIsStatic, myInitialization);
  }
}