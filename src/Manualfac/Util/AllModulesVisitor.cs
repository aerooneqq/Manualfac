using Microsoft.CodeAnalysis;

namespace Manualfac.Util;

public static class AllModulesVisitor
{
  public static void Visit(
    Compilation compilation, bool processCompilationModule, Func<IModuleSymbol, bool> actionWithModule)
  {
    var modulesQueue = new Queue<IModuleSymbol>();
    var visited = new HashSet<IModuleSymbol>(SymbolEqualityComparer.Default);

    if (processCompilationModule)
    {
      modulesQueue.Enqueue(compilation.SourceModule); 
    }
    else
    {
      ExecuteWithModuleReferences(compilation.SourceModule, refModule =>
      {
        modulesQueue.Enqueue(refModule);
      });
    }

    while (modulesQueue.Count != 0)
    {
      var module = modulesQueue.Dequeue();
      visited.Add(module);

      var shouldStop = actionWithModule(module);

      if (shouldStop) return;
      
      ExecuteWithModuleReferences(module, refAsmModule =>
      {
        if (!visited.Contains(refAsmModule))
        {
          modulesQueue.Enqueue(refAsmModule);
        }
      });
    }
  }

  public static void ExecuteWithModuleReferences(IModuleSymbol module, Action<IModuleSymbol> refModuleAction)
  {
    foreach (var refAsm in module.ReferencedAssemblySymbols)
    {
      foreach (var refAsmModule in refAsm.Modules)
      {
        refModuleAction(refAsmModule);
      }
    }
  }
}