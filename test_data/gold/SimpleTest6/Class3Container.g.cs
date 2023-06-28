using DefaultNamespace;
using System.Threading;

namespace DefaultNamespace 
{
  public partial class Class3Container
  {
    private static DefaultNamespace.Class3 ourInstance;
    private static object ourSync = new object();

    public static DefaultNamespace.Class3 Resolve()
    {
      if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;
      lock (ourSync)
      {
        if (Volatile.Read(ref ourInstance) is { } exiting2) return exiting2;
        var created = new DefaultNamespace.Class3(
          new DefaultNamespace.IClass1[] {Class1Container.Resolve(),Class11Container.Resolve(),Class111Container.Resolve(),Class1111Container.Resolve()}
        );
        Volatile.Write(ref ourInstance, created);
        return created;
      }
    }
  }
}