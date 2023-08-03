using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions.BeforeAfterRelation;

public class ReferenceToOtherInterfaceComponentException(
    INamedTypeSymbol interfaceSymbol, INamedTypeSymbol component, INamedTypeSymbol reference)
  : ManualfacGeneratorException
{
  public override string Message { get; } = $"{ExtensionsForINamedTypeSymbol.GetFullName(component)} (impl of {ExtensionsForINamedTypeSymbol.GetFullName(interfaceSymbol)}) " +
                                            $"can not reference {ExtensionsForINamedTypeSymbol.GetFullName(reference)} in before-after relation";
}