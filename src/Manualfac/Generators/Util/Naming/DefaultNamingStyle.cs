namespace Manualfac.Generators.Util.Naming;

public class DefaultNamingStyle : NamingStyle
{
  public static DefaultNamingStyle Instance { get; } = new();


  private DefaultNamingStyle()
  {
  }


  public override string ApplyNamingStyleTo(string originalFieldName) => originalFieldName;
}