using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class OverrideMustBeClassException(INamedTypeSymbol componentSymbol, INamedTypeSymbol overrideSymbol)
  : ManualfacGeneratorException
{
  public override string Message { get; } =
    $"Component {componentSymbol.GetFullName()} can not override {overrideSymbol.GetFullName()}";
}