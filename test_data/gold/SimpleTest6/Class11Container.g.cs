using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class11Container
  {
    private static DefaultNamespace.Class11 ourInstance;
    private static object ourSync = new object();
    private static Func<DefaultNamespace.Class11> ourF = DefaultInitialize;

    public static DefaultNamespace.Class11 Resolve(

    )
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = ourF();
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }

    public static void Initialize(
      Func<DefaultNamespace.Class11> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static DefaultNamespace.Class11 DefaultInitialize(

    )
    {
      var created = new DefaultNamespace.Class11(

      );
      return created;
    }


  }
}