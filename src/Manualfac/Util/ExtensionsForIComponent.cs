using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Util;

internal static class ExtensionsForIComponent
{
  public static IReadOnlyList<(AttributeSyntax Node, IReadOnlyList<INamedTypeSymbol> Types)> GetAttributesWithTypeArguments(
    this ISymbol symbol, INamedTypeSymbol attributeSymbol)
  {
    return symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.ConstructedFrom.IsSubTypeOf(attributeSymbol) ?? false)
      .Where(attr => attr.ApplicationSyntaxReference is { })
      .Select(attr => (Attribute: attr, Node: attr.ApplicationSyntaxReference!.GetSyntax() as AttributeSyntax))
      .Where(pair => pair.Node is { })
      .Select(pair => (
        pair.Node!, 
        (IReadOnlyList<INamedTypeSymbol>)pair.Attribute.AttributeClass!.TypeArguments.OfType<INamedTypeSymbol>().ToList()
      ))
      .ToList();
  }
  
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