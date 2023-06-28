using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class3Container
  {
    private static Class3 ourInstance;
    private static object ourSync = new object();

    public static Class3 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new Class3(
          Class1Container.Resolve()
        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}