namespace Manualfac.Util.Naming;

public class PrefixSuffixNamingStyle(string prefix, string suffix) : NamingStyle
{
  public override string ApplyNamingStyleTo(string originalFieldName)
  {
    return prefix + originalFieldName + suffix;
  }
}