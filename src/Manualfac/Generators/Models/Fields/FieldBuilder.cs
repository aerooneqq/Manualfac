using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Fields;

internal class FieldBuilder
{
  private readonly string myTypeName;
  private readonly string myFieldName;

  private bool myIsStatic;
  private bool myReadonly;
  private string? myInitialization;
  private AccessModifier myAccessModifier = AccessModifier.Private;


  public FieldBuilder(string typeName, string fieldName)
  {
    myTypeName = typeName;
    myFieldName = fieldName;
  }


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
    return new GeneratedFieldModel(myTypeName, myFieldName, myAccessModifier, myReadonly, myIsStatic, myInitialization);
  }
}