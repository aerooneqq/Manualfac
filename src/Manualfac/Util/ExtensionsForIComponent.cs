using Microsoft.CodeAnalysis;

namespace Manualfac.Util;

internal static class ExtensionsForIComponent
{
  public static IReadOnlyList<IReadOnlyList<INamedTypeSymbol>> GetAttributesTypeArgumentsByLevels(
    this ISymbol symbol, INamedTypeSymbol attributeSymbol)
  {
    return symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.ConstructedFrom.IsSubTypeOf(attributeSymbol) ?? false)
      .Select(attr => attr.AttributeClass!.TypeArguments.OfType<INamedTypeSymbol>().ToList())
      .ToList();
  }

  public static IReadOnlyList<INamedTypeSymbol> GetAttributesTypeArguments(
    this ISymbol symbol, INamedTypeSymbol attributeSymbol)
  {
    return symbol.GetAttributesTypeArgumentsByLevels(attributeSymbol).SelectMany(attrs => attrs).ToList();
  }
}