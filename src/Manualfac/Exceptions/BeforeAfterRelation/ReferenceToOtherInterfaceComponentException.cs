using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions.BeforeAfterRelation;

public class ReferenceToOtherInterfaceComponentException : ManualfacGeneratorException
{
  public override string Message { get; }

  
  public ReferenceToOtherInterfaceComponentException(
    INamedTypeSymbol interfaceSymbol, INamedTypeSymbol component, INamedTypeSymbol reference)
  {
    Message = $"{component.GetFullName()} (impl of {interfaceSymbol.GetFullName()}) " +
              $"can not reference {reference.GetFullName()} in before-after relation";
  }
}