using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class NoImplementationForInterfaceException : ManualfacGeneratorException
{
  public override string Message { get; }


  public NoImplementationForInterfaceException(INamedTypeSymbol interfaceSymbol)
  {
    Debug.Assert(interfaceSymbol.TypeKind == TypeKind.Interface);
    Message = $"Failed to find implementation for {interfaceSymbol.Name}";
  }
}