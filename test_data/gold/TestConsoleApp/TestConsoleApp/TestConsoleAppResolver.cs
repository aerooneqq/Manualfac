public partial class TestConsoleAppResolver
{
  internal static TComponent ResolveOrThrow<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(ClassLibrary2.Class4))
    {
      return (TComponent)((object)ClassLibrary2.Class4Container.Resolve());
    }
    if (typeof(TComponent) == typeof(ClassLibrary2.Class5))
    {
      return (TComponent)((object)ClassLibrary2.Class5Container.Resolve());
    }
    if (typeof(TComponent) == typeof(ClassLibrary1.Class3))
    {
      return (TComponent)((object)ClassLibrary1.Class3Container.Resolve());
    }
    if (typeof(TComponent) == typeof(Class6))
    {
      return (TComponent)((object)Class6Container.Resolve());
    }
    if (typeof(TComponent) == typeof(ClassLibrary1.Class2))
    {
      return (TComponent)((object)ClassLibrary1.Class2Container.Resolve());
    }
    if (typeof(TComponent) == typeof(ClassLibrary1.Class1))
    {
      return (TComponent)((object)ClassLibrary1.Class1Container.Resolve());
    }
    throw new ArgumentOutOfRangeException();
  }

  internal static IEnumerable<TComponent> ResolveComponentsOrThrow<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(ClassLibrary2.IInterface))
    {
      return new TComponent[] {(TComponent)((object)ClassLibrary2.Class4Container.Resolve()),(TComponent)((object)ClassLibrary2.Class5Container.Resolve()),(TComponent)((object)Class6Container.Resolve()),(TComponent)((object)ClassLibrary1.Class1Container.Resolve())};
    }
    throw new ArgumentOutOfRangeException();
  }


}