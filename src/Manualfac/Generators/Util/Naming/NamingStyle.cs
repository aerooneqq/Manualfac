namespace Manualfac.Generators.Util.Naming;

public abstract class NamingStyle
{
  public static NamingStyle ParseOrDefault(string style) => new PrefixNamingStyle(style);

  public abstract string CreateFieldName(string originalFieldName);
}