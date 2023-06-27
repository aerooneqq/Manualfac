using System.Diagnostics;
using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class CantResolveConcreteImplementationException : ManualfacGeneratorException
{
  public override string Message { get; }


  public CantResolveConcreteImplementationException(INamedTypeSymbol interfaceSymbol, IReadOnlyList<IComponentInfo> impls)
  {
    Debug.Assert(interfaceSymbol.TypeKind == TypeKind.Interface);
    Message = $"Expected that {interfaceSymbol.Name} has one implementation, instead it has [{string.Join(",", impls.Select(impl => impl.FullName))}]";
  }
}