using Microsoft.CodeAnalysis;

namespace Manualfac.Components.Caches;

internal class ComponentAndNonComponentSymbols(ManualfacSymbols manualfacSymbols)
{
  private readonly HashSet<INamedTypeSymbol> myComponentsSymbols = new(SymbolEqualityComparer.Default);
  private readonly HashSet<INamedTypeSymbol> myNotComponentsSymbols = new(SymbolEqualityComparer.Default);


  public bool CheckIfManualfacComponent(INamedTypeSymbol symbol)
  {
    if (myNotComponentsSymbols.Contains(symbol)) return false;
    if (myComponentsSymbols.Contains(symbol)) return true;

    if (symbol.GetAttributes().Any(IsManualfacAttribute))
    {
      myComponentsSymbols.Add(symbol);

      return true;
    }

    myNotComponentsSymbols.Add(symbol);

    return false;
  }

  private bool IsManualfacAttribute(AttributeData attribute)
  {
    var attributeClass = attribute.AttributeClass;
    while (attributeClass is { })
    {
      if (SymbolEqualityComparer.Default.Equals(attributeClass, manualfacSymbols.ComponentAttribute))
      {
        return true;
      }

      attributeClass = attributeClass.BaseType;
    }

    return false;
  }
}