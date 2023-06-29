using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class OverrideMustBeClassException : ManualfacGeneratorException
{
  public override string Message { get; }


  public OverrideMustBeClassException(INamedTypeSymbol componentSymbol, INamedTypeSymbol overrideSymbol)
  {
    Message = $"Component {componentSymbol.GetFullName()} can not override {overrideSymbol.GetFullName()}";
  }
}