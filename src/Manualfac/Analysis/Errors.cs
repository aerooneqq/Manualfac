using Manualfac.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Analysis;

public static class Errors
{
  private const string Category = "ManualfacErrors";


  public static Diagnostic DependsOnNonComponentSymbol(
    AttributeSyntax node, INamedTypeSymbol componentType, INamedTypeSymbol dependantType)
  {
    const string ErrorId = "MFAC00001";
    const string Title = "Depending on non-component symbol";

    var componentName = componentType.GetFullName();
    var dependantName = dependantType.GetFullName();
    var text = $"Can not reference non-component symbol (component: {componentName}, dependency: {dependantName})";

    var descriptor = new DiagnosticDescriptor(ErrorId, Title, text, Category, DiagnosticSeverity.Error, true);
    return Diagnostic.Create(descriptor, node.GetLocation());
  }

  public static Diagnostic DuplicatedDependency(
    AttributeSyntax node, INamedTypeSymbol componentType, INamedTypeSymbol duplicatedDependency)
  {
    const string ErrorId = "MFAC00002";
    const string Title = "Duplicated dependency symbol";

    var componentName = componentType.GetFullName();
    var duplicateName = duplicatedDependency.GetFullName();
    var text = $"Duplicated dependency (component: {componentName}, duplicated dependency: {duplicateName})";

    var descriptor = new DiagnosticDescriptor(ErrorId, Title, text, Category, DiagnosticSeverity.Error, true);
    return Diagnostic.Create(descriptor, node.GetLocation());
  }
}