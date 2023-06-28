using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class5Container
  {
    private static DefaultNamespace.Class5 ourInstance;
    private static object ourSync = new object();

    public static DefaultNamespace.Class5 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new DefaultNamespace.Class5(
          Class1Container.Resolve(),
          Class2Container.Resolve(),
          Class3Container.Resolve(),
          Class4Container.Resolve()
        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}