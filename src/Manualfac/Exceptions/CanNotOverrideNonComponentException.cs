using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class CanNotOverrideNonComponentException(
  INamedTypeSymbol overrideSymbol, 
  INamedTypeSymbol baseSymbol
) : ManualfacGeneratorException
{
  public override string Message { get; } = 
    $"{overrideSymbol.GetFullName()} can not override non-component {baseSymbol.GetFullName()}";
}