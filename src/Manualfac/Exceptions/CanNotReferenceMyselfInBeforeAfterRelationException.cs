using Manualfac.Generators.Components;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class CanNotReferenceMyselfInBeforeAfterRelationException : ManualfacGeneratorException
{
  public override string Message { get; }


  public CanNotReferenceMyselfInBeforeAfterRelationException(INamedTypeSymbol componentSymbol)
  {
    Message = $"{componentSymbol.GetFullName()} can not reference itself in before-after relation";
  }
}