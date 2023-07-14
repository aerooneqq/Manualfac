using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Caches;

internal class ComponentAndNonComponentSymbols
{
  private readonly ManualfacSymbols myManualfacSymbols;
  private readonly HashSet<INamedTypeSymbol> myComponentsSymbols;
  private readonly HashSet<INamedTypeSymbol> myNotComponentsSymbols;


  public ComponentAndNonComponentSymbols(ManualfacSymbols manualfacSymbols)
  {
    myManualfacSymbols = manualfacSymbols;
    myComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myNotComponentsSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
  }
  
  
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
    if (attributeClass is { })
    {
      return SymbolEqualityComparer.Default.Equals(attributeClass, myManualfacSymbols.ComponentAttribute);
    }

    return false;
  }
}