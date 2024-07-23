namespace Manualfac.Util;

internal static class Constants
{
  public const string Global = "global::";
  public const string Manualfac = "Manualfac";

  public const string ManualfacAttribute = "ManualfacAttribute";
  public const string GenerateResolverAttribute = "GenerateResolverAttribute";
  public const string ComponentAttribute = "ComponentAttribute";
  public const string OverridesAttribute = "OverridesAttribute";
  public const string AsAttributeBase = "AsAttributeBase";
  public const string DependsOnAttributeBase = "DependsOnAttributeBase";
  public const string AfterAttributeBase = "AfterAttributeBase";
  public const string BeforeAttributeBase = "BeforeAttributeBase";
  public const string ManualInitializationAttribute = "ManualInitializationAttribute";

  public const string ManualfacAttributeFullName = $"{Global}{ManualfacAttributes}.{ManualfacAttribute}";
  public const string GenerateResolverAttributeFullName = $"{Global}{ManualfacAttributes}.{GenerateResolverAttribute}";
  public const string ComponentAttributeFullName = $"{Global}{ManualfacAttributes}.{ComponentAttribute}";
  public const string OverridesAttributeFullName = $"{Global}{ManualfacAttributes}.{OverridesAttribute}<TComponent>";
  public const string AsAttributeBaseFullName = $"{Global}{ManualfacAttributes}.{AsAttributeBase}";
  public const string DependsOnAttributeBaseFullName = $"{Global}{ManualfacAttributes}.{DependsOnAttributeBase}";
  public const string AfterAttributeBaseFullName = $"{Global}{ManualfacAttributes}.{AfterAttributeBase}";
  public const string BeforeAttributeBaseFullName = $"{Global}{ManualfacAttributes}.{BeforeAttributeBase}";
  public const string ManualInitializationAttributeFullName = $"{Global}{ManualfacAttributes}.{ManualInitializationAttribute}";

  public const string ManualfacAttributes = "ManualfacAttributes";

  public const string GenericIEnumerable = "IEnumerable`1";

  public const string ResolveMethod = "Resolve";
  public const string InitializeMethod = "Initialize";
}