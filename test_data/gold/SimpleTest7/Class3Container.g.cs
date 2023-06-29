using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class3Container
  {
    private static DefaultNamespace.Class3 ourInstance;
    private static object ourSync = new object();

    public static DefaultNamespace.Class3 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new DefaultNamespace.Class3(
          Class2Container.Resolve(),
          Class1Container.Resolve()
        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}