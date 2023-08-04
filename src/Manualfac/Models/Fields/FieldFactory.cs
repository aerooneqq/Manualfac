namespace Manualfac.Models.Fields;

internal static class FieldFactory
{
  internal static GeneratedFieldModel PrivateStaticReadonly(string typeName, string fieldName)
  {
    return new FieldBuilder(typeName, fieldName).Readonly(true).Static(true).Build();
  }

  internal static GeneratedFieldModel PrivateStatic(string typeName, string fieldName)
  {
    return new FieldBuilder(typeName, fieldName).Readonly(false).Static(true).Build();
  }

  internal static GeneratedFieldModel PrivateStaticInitialized(string typeName, string fieldName, string initialization)
  {
    return new FieldBuilder(typeName, fieldName).Readonly(false).Static(true).Initialization(initialization).Build();
  }
}