using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class ComponentShouldSpecifyOnlyOneConstructorException : ManualfacGeneratorException
{
  public override string Message { get; }


  public ComponentShouldSpecifyOnlyOneConstructorException(INamedTypeSymbol symbol)
  {
    Message = $"Type {symbol.Name} declared {symbol.Constructors.Length} constructors, when only one is expected";
  }
}