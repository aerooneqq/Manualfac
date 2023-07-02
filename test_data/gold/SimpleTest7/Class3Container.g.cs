using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class3Container
  {
    private static DefaultNamespace.Class3 ourInstance;
    private static object ourSync = new object();
    private static Func<DefaultNamespace.Class3> ourF = DefaultInitialize;

    public static DefaultNamespace.Class3 Resolve(

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
      Func<DefaultNamespace.Class3> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static DefaultNamespace.Class3 DefaultInitialize(

    )
    {
      var created = new DefaultNamespace.Class3(
        DefaultNamespace.Class2Container.Resolve(),
        DefaultNamespace.Class1Container.Resolve()
      );
      return created;
    }


  }
}