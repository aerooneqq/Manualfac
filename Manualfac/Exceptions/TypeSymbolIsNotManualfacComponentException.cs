using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class TypeSymbolIsNotManualfacComponentException : ManualfacGeneratorException
{
  public override string Message { get; }
  
  
  public TypeSymbolIsNotManualfacComponentException(INamedTypeSymbol symbol)
  {
    Message = $"Type {symbol.Name} is not a Manualfac component";
  }
}