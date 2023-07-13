using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

internal static class ExtensionsForINamedTypeSymbol
{
  public static string GetFullName(this INamedTypeSymbol symbol) => symbol.ToString();

  public static string GetAssociatedFieldNameBase(this INamedTypeSymbol symbol)
  {
    var name = symbol.Name;
    return symbol.TypeKind switch
    {
      TypeKind.Interface => name.Substring(1),
      _ => name
    };
  }
}