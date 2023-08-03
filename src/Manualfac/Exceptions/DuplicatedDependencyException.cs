using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class DuplicatedDependencyException(INamedTypeSymbol componentSymbol, INamedTypeSymbol duplicatedDependency)
  : ManualfacGeneratorException
{
  public override string Message { get; } =
    $"{duplicatedDependency.GetFullName()} dependency was duplicated in {componentSymbol.GetFullName()}";
}