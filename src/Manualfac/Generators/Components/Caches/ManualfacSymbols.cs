using System.Reflection.Metadata;
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
  

  private readonly IModuleSymbol myManualfacModule;


  public INamedTypeSymbol ManualfacAttribute => FindTypeOrThrow(Constants.ManualfacAttribute);
  public INamedTypeSymbol ComponentAttribute => FindTypeOrThrow(Constants.ComponentAttribute);
  public INamedTypeSymbol DependsOnAttributeBase => FindTypeOrThrow(Constants.DependsOnAttributeBase);
  public INamedTypeSymbol GenerateResolverAttribute => FindTypeOrThrow(Constants.GenerateResolverAttribute);
  public INamedTypeSymbol OverridesAttribute => FindTypeOrThrow(Constants.OverridesAttribute);
  public INamedTypeSymbol AsAttributeBase => FindTypeOrThrow(Constants.AsAttributeBase);


  private ManualfacSymbols(IModuleSymbol manualfacModule)
  {
    myManualfacModule = manualfacModule;
  }


  public INamedTypeSymbol FindTypeOrThrow(string name)
  {
    var foundSymbol = myManualfacModule.GetTypes().FirstOrDefault(type => type.Name == name);
    if (foundSymbol is null)
    {
      throw new ArgumentOutOfRangeException(name);
    }

    return foundSymbol;
  }
}