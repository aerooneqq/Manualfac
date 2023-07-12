using ClassLibrary2;
using System.Threading;

public partial class Class6Container
{
  private static Class6 ourInstance;
  private static object ourSync = new object();
  private static Func<Class6> ourF = DefaultInitialize;

  public static Class6 Resolve(

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
    Func<Class6> f
  )
  {
    lock (ourSync)
    {
      ourF=f;
    }
  }

  private static Class6 DefaultInitialize(

  )
  {
    var created = new Class6(
      ClassLibrary2.Class4Container.Resolve(),
      ClassLibrary2.Class4Container.Resolve(),
      ClassLibrary2.Class5Container.Resolve()
    );
    return created;
  }


}