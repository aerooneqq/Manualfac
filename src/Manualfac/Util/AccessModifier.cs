namespace Manualfac.Util;

public enum AccessModifier
{
  Public,
  Private,
  Protected,
  Internal,
  PrivateProtected,
  ProtectedInternal
}

public static class AccessModifierExtensions
{
  public static string CreateModifierString(this AccessModifier modifier) => modifier switch
  {
    AccessModifier.Public => "public",
    AccessModifier.Private => "private",
    AccessModifier.Protected => "protected",
    AccessModifier.Internal => "internal",
    AccessModifier.PrivateProtected => "private protected",
    AccessModifier.ProtectedInternal => "protected internal",
    _ => throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null)
  };
}