using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class11Container
  {
    private static DefaultNamespace.Class11 ourInstance;
    private static object ourSync = new object();

    public static DefaultNamespace.Class11 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new DefaultNamespace.Class11(

        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}