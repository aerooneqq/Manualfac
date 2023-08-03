using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class ComponentShouldSpecifyOnlyOneConstructorException(INamedTypeSymbol symbol) : ManualfacGeneratorException
{
  public override string Message { get; } = 
    $"Type {symbol.Name} declared {symbol.Constructors.Length} constructors, when only one is expected";
}