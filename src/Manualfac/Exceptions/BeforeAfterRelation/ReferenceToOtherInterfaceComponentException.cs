using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions.BeforeAfterRelation;

public class ReferenceToOtherInterfaceComponentException(
    INamedTypeSymbol interfaceSymbol, INamedTypeSymbol component, INamedTypeSymbol reference)
  : ManualfacGeneratorException
{
  public override string Message { get; } = $"{component.GetFullName()} (impl of {interfaceSymbol.GetFullName()}) " +
                                            $"can not reference {reference.GetFullName()} in before-after relation";
}