using ClassLibrary1;
using ClassLibrary2;
using System.Threading;

namespace ClassLibrary1 
{
  public partial class Class1Container
  {
    private static ClassLibrary1.Class1 ourInstance;
    private static object ourSync = new object();
    private static Func<ClassLibrary1.Class1> ourF = DefaultInitialize;

    public static ClassLibrary1.Class1 Resolve(

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
      Func<ClassLibrary1.Class1> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static ClassLibrary1.Class1 DefaultInitialize(

    )
    {
      var created = new ClassLibrary1.Class1(
        ClassLibrary1.Class2Container.Resolve(),
        ClassLibrary1.Class3Container.Resolve(),
        ClassLibrary2.Class4Container.Resolve()
      );
      return created;
    }


  }
}