using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class4Container
  {
    private static DefaultNamespace.Class4 ourInstance;
    private static object ourSync = new object();
    private static Func<DefaultNamespace.Class4> ourF = DefaultInitialize;

    public static DefaultNamespace.Class4 Resolve(

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
      Func<DefaultNamespace.Class4> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static DefaultNamespace.Class4 DefaultInitialize(

    )
    {
      var created = new DefaultNamespace.Class4(
        DefaultNamespace.Class3Container.Resolve(),
        DefaultNamespace.Class2Container.Resolve(),
        DefaultNamespace.Class1Container.Resolve()
      );
      return created;
    }


  }
}