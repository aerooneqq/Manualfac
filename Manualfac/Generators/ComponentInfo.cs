using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

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

internal class ComponentInfo
{
  private readonly IReadOnlyList<(ComponentInfo Component, AccessModifier Modifier)> myOrderedDependencies;

  public INamedTypeSymbol ComponentSymbol { get; }
  public HashSet<ComponentInfo> Dependencies { get; }


  public string ShortName => ComponentSymbol.Name;
  public string FullName => Namespace is { } @namespace ? @namespace + "." + ShortName : ShortName;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => ComponentSymbol.ContainingNamespace.Name;

  
  public ComponentInfo(
    INamedTypeSymbol componentSymbol, 
    IReadOnlyCollection<(ComponentInfo Component, AccessModifier Modifier)> dependencies)
  {
    ComponentSymbol = componentSymbol;
    Dependencies = new HashSet<ComponentInfo>(dependencies.Select(dep => dep.Component));
    myOrderedDependencies = dependencies.ToList();
  }


  public IReadOnlyList<(ComponentInfo Component, AccessModifier Modifier)> GetOrCreateOrderedListOfDependencies()
  {
    return myOrderedDependencies;
  }
}