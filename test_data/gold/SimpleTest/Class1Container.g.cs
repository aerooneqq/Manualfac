using System.Threading;

namespace asd 
{
  public partial class Class1Container
  {
    private static Class1 ourInstance;
    private static object ourSync = new object();

    public static Class1 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new Class1(

        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}