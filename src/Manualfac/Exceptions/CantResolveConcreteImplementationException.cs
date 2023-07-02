using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class CantResolveConcreteImplementationException : ManualfacGeneratorException
{
  public override string Message { get; }


  public CantResolveConcreteImplementationException(INamedTypeSymbol interfaceSymbol, IReadOnlyList<string> impls)
  {
    Debug.Assert(interfaceSymbol.TypeKind == TypeKind.Interface);
    Message = $"Expected that {interfaceSymbol.Name} has one implementation, instead it has [{string.Join(",", impls)}]";
  }
}