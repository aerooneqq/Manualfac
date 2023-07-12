using System.Threading;

namespace ClassLibrary2 
{
  public partial class Class4Container
  {
    private static ClassLibrary2.Class4 ourInstance;
    private static object ourSync = new object();
    private static Func<ClassLibrary2.Class4> ourF = DefaultInitialize;

    public static ClassLibrary2.Class4 Resolve(

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
      Func<ClassLibrary2.Class4> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static ClassLibrary2.Class4 DefaultInitialize(

    )
    {
      var created = new ClassLibrary2.Class4(

      );
      return created;
    }


  }
}