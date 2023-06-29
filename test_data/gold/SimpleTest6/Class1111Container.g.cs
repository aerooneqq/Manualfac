using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class1111Container
  {
    private static DefaultNamespace.Class1111 ourInstance;
    private static object ourSync = new object();
    private static Func<DefaultNamespace.Class1111> ourF = DefaultInitialize;

    public static DefaultNamespace.Class1111 Resolve(

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
      Func<DefaultNamespace.Class1111> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static DefaultNamespace.Class1111 DefaultInitialize(

    )
    {
      var created = new DefaultNamespace.Class1111(

      );
      return created;
    }


  }
}