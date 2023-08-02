using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class5Container
  {
    private static DefaultNamespace.Class5 ourInstance;
    private static object ourSync = new object();
    private static Func<DefaultNamespace.Class5> ourF = DefaultInitialize;

    public static DefaultNamespace.Class5 Resolve(

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
      Func<DefaultNamespace.Class5> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static DefaultNamespace.Class5 DefaultInitialize(

    )
    {
      var created = new DefaultNamespace.Class5(

      );
      return created;
    }


  }
}