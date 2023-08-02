using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class TooManyOverridesException : ManualfacGeneratorException
{
  public override string Message { get; }


  public TooManyOverridesException(INamedTypeSymbol symbol, IReadOnlyList<INamedTypeSymbol> overrides)
  {
    var overridesString = string.Join(",", overrides.Select(o => o.GetFullName()));
    Message = $"Type {symbol.GetFullName()} has too many overrides: {overridesString}";
  }
}