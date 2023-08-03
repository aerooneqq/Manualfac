using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions.BeforeAfterRelation;

public class SelfReferenceInBeforeAfterRelationException(INamedTypeSymbol componentSymbol) : ManualfacGeneratorException
{
  public override string Message { get; } = $"{componentSymbol.GetFullName()} can not reference itself in before-after relation";
}