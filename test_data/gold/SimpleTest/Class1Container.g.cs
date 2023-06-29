using System.Threading;

namespace asd 
{
  public partial class Class1Container
  {
    private static asd.Class1 ourInstance;
    private static object ourSync = new object();
    private static Func<asd.Class1> ourF = DefaultInitialize;

    public static asd.Class1 Resolve(

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
      Func<asd.Class1> f
    )
    {
      lock (ourSync)
      {
        ourF=f;
      }
    }

    private static asd.Class1 DefaultInitialize(

    )
    {
      var created = new asd.Class1(

      );
      return created;
    }


  }
}