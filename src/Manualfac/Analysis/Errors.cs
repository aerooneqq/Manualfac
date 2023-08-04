using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Analysis;

public static class Errors
{
  private const string Category = "ManualfacErrors";
  
  public static Diagnostic DependsOnNonComponentSymbol(AttributeSyntax node)
  {
    const string ErrorId = "MFAC00001";
    const string Title = "Depending on non-component symbol";

    var text = "Can not reference non-component symbol";
    var descriptor = new DiagnosticDescriptor(ErrorId, Title, text, Category, DiagnosticSeverity.Error, true);
    return Diagnostic.Create(descriptor, node.GetLocation());
  }
}