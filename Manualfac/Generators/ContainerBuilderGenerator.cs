using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal static class ContainerBuilderGenerator
{
  public static void GenerateContainerInitialization(
    ComponentInfoStorage componentsStorage, GeneratorExecutionContext context)
  {
    foreach (var componentInfo in componentsStorage.GetInTopologicalOrder())
    {
    }
  }
}