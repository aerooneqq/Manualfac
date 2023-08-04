using System.Text;

namespace Manualfac.Models.Methods;

internal static class MethodFactory
{
  internal static GeneratedMethodModel PublicStatic(
    string name, string returnTypeName, Action<StringBuilder, int> methodGenerator)
  {
    return new MethodBuilder(name, returnTypeName, methodGenerator).Public().Static(true).Build();
  }

  internal static GeneratedMethodModel PrivateStatic(
    string name, string returnTypeName, Action<StringBuilder, int> methodGenerator)
  {
    return new MethodBuilder(name, returnTypeName, methodGenerator).Private().Static(true).Build();
  }

  internal static GeneratedMethodModel PublicStaticParameters(
    string name,
    string returnTypeName,
    Action<StringBuilder, int> methodGenerator,
    IReadOnlyList<GeneratedParameterModel> @params)
  {
    return new MethodBuilder(name, returnTypeName, methodGenerator).Public().Static(true).Parameters(@params).Build();
  }

  internal static GeneratedMethodModel InternalStaticGeneric(
    string name, string returnTypeName, IReadOnlyList<string> typeParams, Action<StringBuilder, int> methodGenerator)
  {
    return new MethodBuilder(name, returnTypeName, methodGenerator)
      .Internal().Static(true).TypeParams(typeParams)
      .Build();
  }

  internal static GeneratedMethodModel InternalStatic(
    string name, string returnTypeName, Action<StringBuilder, int> methodGenerator)
  {
    return new MethodBuilder(name, returnTypeName, methodGenerator).Internal().Static(true).Build();
  }
}