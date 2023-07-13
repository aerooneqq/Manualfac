namespace Manualfac.Generators.Util.Naming;

public class PrefixSuffixNamingStyle : NamingStyle
{
  private readonly string myPrefix;
  private readonly string mySuffix;


  public PrefixSuffixNamingStyle(string prefix, string suffix)
  {
    myPrefix = prefix;
    mySuffix = suffix;
  }


  public override string CreateFieldName(string originalFieldName)
  {
    return myPrefix + originalFieldName + mySuffix;
  }
}