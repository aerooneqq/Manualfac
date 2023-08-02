using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class CantResolveConcreteImplementationException : ManualfacGeneratorException
{
  public override string Message { get; }


  public CantResolveConcreteImplementationException(INamedTypeSymbol symbol, IReadOnlyList<string> impls)
  {
    Debug.Assert(symbol.TypeKind == TypeKind.Interface);
    Message = $"Expected that {symbol.Name} has one implementation, instead it has [{string.Join(",", impls)}]";
  }
}