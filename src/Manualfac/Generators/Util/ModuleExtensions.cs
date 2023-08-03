using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

internal static class ModuleExtensions
{
  public static IEnumerable<INamedTypeSymbol> GetTypes(this IModuleSymbol module)
  {
    return GetAllNamespacesFrom(module.GlobalNamespace).SelectMany(ns => ns.GetTypeMembers());
  }

  private static IEnumerable<INamespaceSymbol> GetAllNamespacesFrom(INamespaceSymbol namespaceSymbol)
  {
    yield return namespaceSymbol;

    foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
    {
      foreach (var nextNamespace in GetAllNamespacesFrom(childNamespace))
      {
        yield return nextNamespace;
      }
    }
  }
}