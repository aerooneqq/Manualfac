using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class FailedToFindManualfacAttributesModuleException(Compilation compilation) : ManualfacGeneratorException
{
  public override string Message { get; } = $"Failed to find ManualfacAttributes in {compilation.AssemblyName}";
}