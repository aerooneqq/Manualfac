namespace Manualfac.Generators.Models;

internal enum GeneratedClassAccessModifier
{
  Public,
  Internal,
  File
}

internal static class GeneratedClassAccessModifierExtensions
{
  public static string CreateModifierString(this GeneratedClassAccessModifier modifier) => modifier switch
  {
    GeneratedClassAccessModifier.Public => "public",
    GeneratedClassAccessModifier.Internal => "internal",
    GeneratedClassAccessModifier.File => "file",
    _ => throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null)
  };
}