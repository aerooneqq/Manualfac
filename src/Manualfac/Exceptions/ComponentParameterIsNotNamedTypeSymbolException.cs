using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class ComponentParameterIsNotNamedTypeSymbolException(IParameterSymbol symbol) : ManualfacGeneratorException
{
  public override string Message { get; } = 
    $"Parameter {symbol.Name} in {symbol.ContainingType.Name} was not of type {nameof(INamedTypeSymbol)}";
}