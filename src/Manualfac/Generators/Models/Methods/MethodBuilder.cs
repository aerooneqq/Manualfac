using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.Methods;

internal class MethodBuilder(string name, string returnType, Action<StringBuilder, int> bodyGenerator)
{
  private bool myIsStatic;
  private bool myIsPartial;
  private IReadOnlyList<string> myTypeParameters = ImmutableList<string>.Empty;
  private IReadOnlyList<GeneratedParameterModel> myParameters = ImmutableList<GeneratedParameterModel>.Empty;
  private AccessModifier myAccessModifier = AccessModifier.Private;


  public MethodBuilder TypeParams(IReadOnlyList<string> typeParams)
  {
    myTypeParameters = typeParams;

    return this;
  }

  public MethodBuilder Parameters(IReadOnlyList<GeneratedParameterModel> parameters)
  {
    myParameters = parameters;

    return this;
  }

  public MethodBuilder Static(bool isStatic)
  {
    myIsStatic = isStatic;

    return this;
  }

  public MethodBuilder Partial(bool isPartial)
  {
    myIsPartial = isPartial;

    return this;
  }

  public MethodBuilder Public()
  {
    myAccessModifier = AccessModifier.Public;

    return this;
  }

  public MethodBuilder Private()
  {
    myAccessModifier = AccessModifier.Private;

    return this;
  }

  public MethodBuilder Internal()
  {
    myAccessModifier = AccessModifier.Internal;

    return this;
  }

  public GeneratedMethodModel Build()
  {
    return new GeneratedMethodModel(
      name, myTypeParameters, returnType, bodyGenerator, myParameters, myAccessModifier, myIsStatic, myIsPartial);
  }
}