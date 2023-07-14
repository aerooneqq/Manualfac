using Manualfac.Exceptions;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Caches;

public class ManualfacSymbols
{
  public static ManualfacSymbols CreateManualfacSymbolsFrom(Compilation compilation)
  {
    IModuleSymbol? manualfacModule = null;
    AllModulesVisitor.Visit(compilation, module =>
    {
      if (module.ContainingAssembly.Name != Constants.ManualfacAttributes) return false;
      
      manualfacModule = module;
      return true;
    });

    if (manualfacModule is null)
    {
      throw new FailedToFindManualfacAttributesModuleException(compilation);
    }

    return new ManualfacSymbols(manualfacModule);
  }
  

  private readonly ModuleTypesCache myTypesCache;


  public INamedTypeSymbol ManualfacAttribute => FindTypeOrThrow(Constants.ManualfacAttributeFullName);
  public INamedTypeSymbol ComponentAttribute => FindTypeOrThrow(Constants.ComponentAttributeFullName);
  public INamedTypeSymbol DependsOnAttributeBase => FindTypeOrThrow(Constants.DependsOnAttributeBaseFullName);
  public INamedTypeSymbol GenerateResolverAttribute => FindTypeOrThrow(Constants.GenerateResolverAttributeFullName);
  public INamedTypeSymbol OverridesAttribute => FindTypeOrThrow(Constants.OverridesAttributeFullName);
  public INamedTypeSymbol AsAttributeBase => FindTypeOrThrow(Constants.AsAttributeBaseFullName);


  private ManualfacSymbols(IModuleSymbol manualfacModule)
  {
    myTypesCache = new ModuleTypesCache(manualfacModule);
  }


  private INamedTypeSymbol FindTypeOrThrow(string name)
  {
    if (myTypesCache.TryGet(name) is not { } type)
    {
      throw new KeyNotFoundException(name);
    }

    return type;
  }
}