using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Util;

public static class AllModulesVisitor
{
  public static void Visit(Compilation compilation, Func<IModuleSymbol, bool> actionWithModule)
  {
    var modulesQueue = new Queue<IModuleSymbol>();
    var visited = new HashSet<IModuleSymbol>(SymbolEqualityComparer.Default);
    
    modulesQueue.Enqueue(compilation.SourceModule);

    while (modulesQueue.Count != 0)
    {
      var module = modulesQueue.Dequeue();
      visited.Add(module);

      var shouldStop = actionWithModule(module);
      if (shouldStop) return;

      foreach (var refAsm in module.ReferencedAssemblySymbols)
      {
        foreach (var refAsmModule in refAsm.Modules)
        {
          if (!visited.Contains(refAsmModule))
          {
            modulesQueue.Enqueue(refAsmModule);
          }
        }
      }
    }
  }
}