using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Methods;

internal static class MethodFactory
{
  internal static GeneratedMethodModel PublicStatic(
    string name, string returnTypeName, Action<StringBuilder, int> methodGenerator)
  {
    return new GeneratedMethodModel(
      name, ImmutableArray<string>.Empty, returnTypeName, methodGenerator,
      ImmutableArray<GeneratedParameterModel>.Empty, isStatic: true);
  }

  internal static GeneratedMethodModel PrivateStatic(
    string name, string returnTypeName, Action<StringBuilder, int> methodGenerator)
  {
    return new GeneratedMethodModel(
      name, ImmutableArray<string>.Empty, returnTypeName, methodGenerator,
      ImmutableArray<GeneratedParameterModel>.Empty, modifier: AccessModifier.Private, isStatic: true);
  }

  internal static GeneratedMethodModel PublicStaticParameters(
    string name,
    string returnTypeName,
    Action<StringBuilder, int> methodGenerator,
    IReadOnlyList<GeneratedParameterModel> parameters)
  {
    return new GeneratedMethodModel(
      name, ImmutableArray<string>.Empty, returnTypeName, methodGenerator, parameters, isStatic: true);
  }

  internal static GeneratedMethodModel InternalStaticGeneric(
    string name, string returnTypeName, IReadOnlyList<string> generics, Action<StringBuilder, int> methodGenerator)
  {
    return new GeneratedMethodModel(
      name,
      generics,
      returnTypeName,
      methodGenerator,
      ImmutableArray<GeneratedParameterModel>.Empty,
      modifier: AccessModifier.Internal,
      isStatic: true);
  }

  internal static GeneratedMethodModel InternalStatic(
    string name, string returnTypeName, Action<StringBuilder, int> methodGenerator)
  {
    return new GeneratedMethodModel(
      name, ImmutableArray<string>.Empty, returnTypeName, methodGenerator,
      ImmutableArray<GeneratedParameterModel>.Empty, modifier: AccessModifier.Internal, isStatic: true);
  }
}