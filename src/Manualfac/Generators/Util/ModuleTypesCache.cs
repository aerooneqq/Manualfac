using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

public class ModuleTypesCache
{
  private readonly IModuleSymbol myModuleSymbol;
  private readonly Dictionary<string, INamedTypeSymbol> myNamesToTypes;

  private bool myIsInitialized;

  
  public ModuleTypesCache(IModuleSymbol moduleSymbol)
  {
    myNamesToTypes = new Dictionary<string, INamedTypeSymbol>();
    myModuleSymbol = moduleSymbol;
  }


  public INamedTypeSymbol? TryGet(string name)
  {
    InitializeIfNeeded();

    return myNamesToTypes.TryGetValue(name, out var type) switch
    {
      true => type,
      false => null
    };
  }

  private void InitializeIfNeeded()
  {
    if (myIsInitialized) return;

    foreach (var type in myModuleSymbol.GetTypes())
    {
      myNamesToTypes[type.GetFullName()] = type;
    }
    
    myIsInitialized = true;
  }
}