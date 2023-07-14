using Microsoft.CodeAnalysis;

namespace Manualfac.Exceptions;

public class FailedToFindManualfacAttributesModuleException : ManualfacGeneratorException
{
  public override string Message { get; }

  
  public FailedToFindManualfacAttributesModuleException(Compilation compilation)
  {
    Message = $"Failed to find ManualfacAttributes in {compilation.AssemblyName}";
  }
}