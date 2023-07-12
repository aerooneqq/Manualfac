namespace Manualfac.Generators.Util.Naming;

public class PrefixNamingStyle : NamingStyle
{
  private readonly string myPrefix;


  public PrefixNamingStyle(string prefix)
  {
    myPrefix = prefix;
  }


  public override string CreateFieldName(string originalFieldName)
  {
    return myPrefix + originalFieldName;
  }
}