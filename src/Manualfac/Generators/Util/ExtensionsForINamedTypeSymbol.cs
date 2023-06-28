using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

internal static class ExtensionsForINamedTypeSymbol
{
  public static string GetFullName(this INamedTypeSymbol symbol) => symbol.ToString();
}