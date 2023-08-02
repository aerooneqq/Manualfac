using Humanizer;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

internal static class ExtensionsForINamedTypeSymbol
{
  public static string GetFullName(this INamedTypeSymbol symbol) => symbol.ToString();

  public static string GetAssociatedFieldNameBase(this INamedTypeSymbol symbol)
  {
    var name = symbol.Name;
    if (name.Length == 1) return name;

    var isCollectionOfComponents = symbol.IsGenericEnumerable();
    if (isCollectionOfComponents)
    {
      symbol = (INamedTypeSymbol)symbol.TypeArguments.First();
      name = symbol.Name;
    }

    name = symbol.TypeKind switch
    {
      TypeKind.Interface => name.Substring(1),
      _ => name
    };

    return isCollectionOfComponents switch
    {
      true => name.Pluralize(),
      false => name
    };
  }

  public static bool IsGenericEnumerable(this INamedTypeSymbol symbol)
  {
    return symbol is { TypeKind: TypeKind.Interface, MetadataName: Constants.GenericIEnumerable };
  }

  public static bool IsSubTypeOf(this INamedTypeSymbol symbol, INamedTypeSymbol other)
  {
    var current = symbol;
    while (current is { })
    {
      if (SymbolEqualityComparer.Default.Equals(current, other))
      {
        return true;
      }

      current = current.BaseType;
    }

    return false;
  }
}