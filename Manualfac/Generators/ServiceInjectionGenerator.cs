using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

[Generator]
public class ServiceInjectionGenerator : ISourceGenerator
{
  public void Initialize(GeneratorInitializationContext context)
  {
  }

  public void Execute(GeneratorExecutionContext context)
  {
    var modulesQueue = new Queue<IModuleSymbol>();
    modulesQueue.Enqueue(context.Compilation.SourceModule);

    while (modulesQueue.Count != 0)
    {
      var module = modulesQueue.Dequeue();

      foreach (var refAsm in module.ReferencedAssemblySymbols)
      {
        foreach (var refAsmModule in refAsm.Modules)
        {
          modulesQueue.Enqueue(refAsmModule);
        }
      }
    }
  }
}