namespace Manualfac.Util.Naming;

public class DefaultNamingStyle : NamingStyle
{
  public static DefaultNamingStyle Instance { get; } = new();


  private DefaultNamingStyle()
  {
  }


  public override string ApplyNamingStyleTo(string originalFieldName) => 
    char.ToLower(originalFieldName[0]) + originalFieldName.Substring(1);
}