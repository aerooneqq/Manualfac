using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class ComponentShouldSpecifyOnlyOneConstructorException : ManualfacGeneratorException
{
  public override string Message { get; }

  
  public ComponentShouldSpecifyOnlyOneConstructorException(INamedTypeSymbol namedTypeSymbol)
  {
    Message = $"Type {namedTypeSymbol.Name} declared {namedTypeSymbol.Constructors.Length} constructors, when only one is expected";
  }
}