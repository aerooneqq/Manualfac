using ClassLibrary2;
using System.Threading;

namespace ClassLibrary2 
{
  public partial class Class5Container
  {
    private static ClassLibrary2.Class5 ourInstance;
    private static object ourSync = new object();
    private static Func<ClassLibrary2.Class5> ourF = DefaultInitialize;

    public static ClassLibrary2.Class5 Resolve(

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
      Func<ClassLibrary2.Class5> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static ClassLibrary2.Class5 DefaultInitialize(

    )
    {
      var created = new ClassLibrary2.Class5(
        ClassLibrary2.Class4Container.Resolve()
      );
      return created;
    }


  }
}