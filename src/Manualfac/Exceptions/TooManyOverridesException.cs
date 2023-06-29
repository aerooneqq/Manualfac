using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class TooManyOverridesException : ManualfacGeneratorException
{
  public override string Message { get; }


  public TooManyOverridesException(INamedTypeSymbol componentSymbol, IReadOnlyList<INamedTypeSymbol> overrides)
  {
    Message = $"Type {componentSymbol.GetFullName()} has too many overrides: {string.Join(",", overrides.Select(o => o.GetFullName()))}";
  }
}