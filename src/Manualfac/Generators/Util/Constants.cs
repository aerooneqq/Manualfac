namespace Manualfac.Generators.Util;

internal static class Constants
{
  public const string Manualfac = "Manualfac";

  public const string ManualfacAttribute = "ManualfacAttribute";
  public const string GenerateResolverAttribute = "GenerateResolverAttribute";
  public const string ComponentAttribute = "ComponentAttribute";
  public const string OverridesAttribute = "OverridesAttribute";
  public const string AsAttributeBase = "AsAttributeBase";
  public const string DependsOnAttributeBase = "DependsOnAttributeBase";
  public const string AfterAttributeBase = "AfterAttributeBase";
  public const string BeforeAttributeBase = "BeforeAttributeBase";
  
  public const string ManualfacAttributeFullName = $"{ManualfacAttributes}.{ManualfacAttribute}";
  public const string GenerateResolverAttributeFullName = $"{ManualfacAttributes}.{GenerateResolverAttribute}";
  public const string ComponentAttributeFullName = $"{ManualfacAttributes}.{ComponentAttribute}";
  public const string OverridesAttributeFullName = $"{ManualfacAttributes}.{OverridesAttribute}<TComponent>";
  public const string AsAttributeBaseFullName = $"{ManualfacAttributes}.{AsAttributeBase}";
  public const string DependsOnAttributeBaseFullName = $"{ManualfacAttributes}.{DependsOnAttributeBase}";
  public const string AfterAttributeBaseFullName = $"{ManualfacAttributes}.{AfterAttributeBase}";
  public const string BeforeAttributeBaseFullName = $"{ManualfacAttributes}.{BeforeAttributeBase}";

  public const string ManualfacAttributes = "ManualfacAttributes";
  
  public const string GenericIEnumerable = "IEnumerable`1";

  public const string ResolveMethod = "Resolve";
  public const string InitializeMethod = "Initialize";
}