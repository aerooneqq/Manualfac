using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Caches;

internal class ComponentAndNonComponentSymbols
{
  private readonly HashSet<INamedTypeSymbol> myComponentsSymbols;
  private readonly HashSet<INamedTypeSymbol> myNotComponentsSymbols;


  public ComponentAndNonComponentSymbols()
  {
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

  private static bool IsManualfacAttribute(AttributeData attribute)
  {
    var attributeClass = attribute.AttributeClass;
    while (attributeClass is { })
    {
      if (attributeClass.Name == "ManualfacAttribute") return true;
      attributeClass = attributeClass.BaseType;
    }

    return false;
  }
}