using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class111Container
  {
    private static DefaultNamespace.Class111 ourInstance;
    private static object ourSync = new object();
    private static Func<DefaultNamespace.Class111> ourF = DefaultInitialize;

    public static DefaultNamespace.Class111 Resolve(

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
      Func<DefaultNamespace.Class111> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static DefaultNamespace.Class111 DefaultInitialize(

    )
    {
      var created = new DefaultNamespace.Class111(

      );
      return created;
    }


  }
}