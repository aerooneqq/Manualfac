using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class CanNotOverrideNonComponentException : ManualfacGeneratorException
{
  public override string Message { get; }


  public CanNotOverrideNonComponentException(INamedTypeSymbol overrideSymbol, INamedTypeSymbol baseSymbol)
  {
    Message = $"{overrideSymbol.GetFullName()} can not override non-component {baseSymbol.GetFullName()}";
  }
}