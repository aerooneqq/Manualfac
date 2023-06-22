using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

public class ParameterIsNotManualfacComponentException : ManualfacGeneratorException
{
  public override string Message { get; }
  
  
  public ParameterIsNotManualfacComponentException(IParameterSymbol parameterSymbol)
  {
    Message = $"Parameter {parameterSymbol.Name} in {parameterSymbol.ContainingType.Name} is not a Manualfac component";
  }
}