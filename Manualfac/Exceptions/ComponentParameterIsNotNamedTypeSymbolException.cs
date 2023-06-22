using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

public class ComponentParameterIsNotNamedTypeSymbolException : ManualfacGeneratorException
{
  public override string Message { get; }

  
  public ComponentParameterIsNotNamedTypeSymbolException(IParameterSymbol symbol)
  {
    Message = $"Parameter {symbol.Name} in {symbol.ContainingType.Name} was not of type {nameof(INamedTypeSymbol)}";
  }
}