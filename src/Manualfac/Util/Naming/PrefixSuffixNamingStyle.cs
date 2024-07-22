namespace Manualfac.Util.Naming;

public class PrefixSuffixNamingStyle(string prefix, string suffix) : NamingStyle
{
  public override string ApplyNamingStyleTo(string originalFieldName)
  {
    var fieldName = prefix.Length switch
    {
      0 => DefaultNamingStyle.Instance.ApplyNamingStyleTo(originalFieldName),
      _ => originalFieldName
    };

    return prefix + fieldName + suffix;
  }
}