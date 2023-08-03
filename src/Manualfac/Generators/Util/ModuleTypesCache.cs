using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

public class ModuleTypesCache
{
  private readonly IModuleSymbol myModuleSymbol;
  private readonly Lazy<Dictionary<string, INamedTypeSymbol>> myNamesToTypes;


  public ModuleTypesCache(IModuleSymbol moduleSymbol)
  {
    myNamesToTypes = new Lazy<Dictionary<string, INamedTypeSymbol>>(InitializeNamesToTypes);
    myModuleSymbol = moduleSymbol;
  }


  public INamedTypeSymbol? TryGet(string name)
  {
    return myNamesToTypes.Value.TryGetValue(name, out var type) switch
    {
      true => type,
      false => null
    };
  }

  private Dictionary<string, INamedTypeSymbol> InitializeNamesToTypes()
  {
    var namesToTypes = new Dictionary<string, INamedTypeSymbol>();
    foreach (var type in myModuleSymbol.GetTypes())
    {
      namesToTypes[type.GetFullName()] = type;
    }

    return namesToTypes;
  }
}