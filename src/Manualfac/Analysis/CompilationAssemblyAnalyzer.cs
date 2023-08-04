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
        anyError |= CheckComponentReferences(type, context);
      }
    }

    return anyError;
  }

  private bool CheckComponentReferences(INamedTypeSymbol type, ManualfacContext context)
  {
    var attributes = type.GetAttributesWithTypeArguments(symbols.DependsOnAttributeBase);
    var anyError = false;
    
    foreach (var (node, references) in attributes)
    {
      foreach (var reference in references.Skip(1))
      {
        if (reference.TypeKind == TypeKind.Class && !storage.CheckIfComponent(reference))
        {
          anyError = true;
          context.ProductionContext.ReportDiagnostic(Errors.DependsOnNonComponentSymbol(node));
        }
      }
    }

    return anyError;
  }
}