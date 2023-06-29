using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class DuplicatedDependencyException : ManualfacGeneratorException
{
  public override string Message { get; }


  public DuplicatedDependencyException(INamedTypeSymbol componentSymbol, INamedTypeSymbol duplicatedDependency)
  {
    Message = $"{duplicatedDependency.GetFullName()} dependency was duplicated in {componentSymbol.GetFullName()}";
  }
}