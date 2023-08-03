using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class TypeSymbolIsNotManualfacComponentException(INamedTypeSymbol symbol) : ManualfacGeneratorException
{
  public override string Message { get; } = $"Type {symbol.Name} is not a Manualfac component";
}