namespace Manualfac.Generators;

public abstract class NamingStyle
{
  public static NamingStyle ParseOrDefault(string style) => new PrefixNamingStyle(style);

  public abstract string CreateFieldName(string originalFieldName);
}

public class DefaultNamingStyle : NamingStyle
{
  public static DefaultNamingStyle Instance { get; } = new();


  private DefaultNamingStyle()
  {
  }


  public override string CreateFieldName(string originalFieldName) => originalFieldName;
}

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
