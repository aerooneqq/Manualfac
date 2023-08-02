namespace Manualfac.Generators.Models.Fields;

internal static class FieldFactory
{
  internal static GeneratedFieldModel PrivateStaticReadonly(string typeName, string fieldName)
  {
    return new GeneratedFieldModel(typeName, fieldName, isStatic: true);
  }

  internal static GeneratedFieldModel PrivateStatic(string typeName, string fieldName)
  {
    return new GeneratedFieldModel(typeName, fieldName, @readonly: false, isStatic: true);
  }

  internal static GeneratedFieldModel PrivateStaticInitialized(string typeName, string fieldName, string initialization)
  {
    return new GeneratedFieldModel(typeName, fieldName, @readonly: false, isStatic: true, defaultValue: initialization);
  }
}