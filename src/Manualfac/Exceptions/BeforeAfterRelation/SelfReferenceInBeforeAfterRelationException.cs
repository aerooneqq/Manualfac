using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions.BeforeAfterRelation;

public class SelfReferenceInBeforeAfterRelationException : ManualfacGeneratorException
{
  public override string Message { get; }


  public SelfReferenceInBeforeAfterRelationException(INamedTypeSymbol componentSymbol)
  {
    Message = $"{componentSymbol.GetFullName()} can not reference itself in before-after relation";
  }
}