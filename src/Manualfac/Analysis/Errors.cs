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
    const string Title = "Depending on non-component symbol";

    var componentName = componentType.GetFullName();
    var dependantName = dependantType.GetFullName();
    var text = $"Can not reference non-component symbol (component: {componentName}, dependency: {dependantName})";

    return CreateDiagnostic(ErrorIds.DependingOnNonComponentSymbolId, Title, text, node);
  }


  private static Diagnostic CreateDiagnostic(string errorId, string title, string text, SyntaxNode node) => 
    Diagnostic.Create(CreateDiagnosticDescriptor(errorId, title, text), node.GetLocation());

  private static DiagnosticDescriptor CreateDiagnosticDescriptor(string errorId, string title, string text) => 
    new(errorId, title, text, Category, DiagnosticSeverity.Error, true);

  public static Diagnostic DuplicatedDependency(
    AttributeSyntax node, INamedTypeSymbol componentType, INamedTypeSymbol duplicatedDependency)
  {
    const string Title = "Duplicated dependency symbol";

    var componentName = componentType.GetFullName();
    var duplicateName = duplicatedDependency.GetFullName();
    var text = $"Duplicated dependency (component: {componentName}, duplicated dependency: {duplicateName})";

    return CreateDiagnostic(ErrorIds.DuplicatedDependencyId, Title, text, node);
  }

  public static Diagnostic MultipleBaseComponents(AttributeSyntax node, INamedTypeSymbol componentType)
  {
    const string Title = "Multiple base components";

    var text = $"Component {componentType.GetFullName()} declared multiple base components";
    return CreateDiagnostic(ErrorIds.MultipleBaseComponentsId, Title, text, node);
  }

  public static Diagnostic BaseComponentIsNotClass(
    AttributeSyntax node, INamedTypeSymbol componentType, INamedTypeSymbol baseType)
  {
    const string Title = "Base component is not a class";

    var text = $"Base component {baseType.GetFullName()} of component {componentType.GetFullName()} is not a class";
    return CreateDiagnostic(ErrorIds.BaseComponentIsNotClassId, Title, text, node);
  }

  public static Diagnostic CanNotOverrideNonComponentSymbol(
    AttributeSyntax node, INamedTypeSymbol component, INamedTypeSymbol baseType)
  {
    const string Title = "Can not override non-component symbol";

    var text = $"Component {component.GetFullName()} can not override non-component symbol {baseType.GetFullName()}";
    return CreateDiagnostic(ErrorIds.CanNotOverrideNonComponentSymbolId, Title, text, node);
  }
}