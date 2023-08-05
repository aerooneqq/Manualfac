using Manualfac.Components;
using Manualfac.Components.Caches;
using Manualfac.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Analysis;

internal class CompilationAssemblyAnalyzer(ManualfacSymbols symbols, ComponentsStorage storage)
{
  public bool ContainsErrors(ManualfacContext context)
  {
    var anyError = false;
    foreach (var module in context.Compilation.Assembly.Modules)
    {
      foreach (var type in module.GetTypes())
      {
        anyError |= CheckComponentDependencies(type, context);
      }
    }

    return anyError;
  }

  private bool CheckComponentDependencies(INamedTypeSymbol type, ManualfacContext context)
  {
    var attributes = type.GetAttributesWithTypeArguments(symbols.DependsOnAttributeBase);
    var anyError = false;

    var seenDependencies = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    foreach (var (node, references) in attributes)
    {
      foreach (var reference in references.Skip(1))
      {
        if (reference.TypeKind == TypeKind.Class && !storage.CheckIfComponent(reference))
        {
          anyError = true;
          context.ProductionContext.ReportDiagnostic(Errors.DependsOnNonComponentSymbol(node, type, reference));
        }

        if (seenDependencies.Contains(reference))
        {
          anyError = true;
          context.ProductionContext.ReportDiagnostic(Errors.DuplicatedDependency(node, type, reference));
        }

        seenDependencies.Add(reference);
      }
    }

    return anyError;
  }
}