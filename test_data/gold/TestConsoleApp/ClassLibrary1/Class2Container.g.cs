using ClassLibrary1;
using System.Threading;

namespace ClassLibrary1 
{
  public partial class Class2Container
  {
    private static ClassLibrary1.Class2 ourInstance;
    private static object ourSync = new object();
    private static Func<ClassLibrary1.Class2> ourF = DefaultInitialize;

    public static ClassLibrary1.Class2 Resolve(

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
      Func<ClassLibrary1.Class2> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static ClassLibrary1.Class2 DefaultInitialize(

    )
    {
      var created = new ClassLibrary1.Class2(
        ClassLibrary1.Class3Container.Resolve()
      );
      return created;
    }


  }
}