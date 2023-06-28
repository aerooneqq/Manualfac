using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class4Container
  {
    private static DefaultNamespace.Class4 ourInstance;
    private static object ourSync = new object();

    public static DefaultNamespace.Class4 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new DefaultNamespace.Class4(
          Class1Container.Resolve(),
          Class2Container.Resolve(),
          Class3Container.Resolve()
        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}