public partial class ClassLibrary2Resolver
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
    throw new ArgumentOutOfRangeException();
  }

  internal static IEnumerable<TComponent> ResolveComponentsOrThrow<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(ClassLibrary2.IInterface))
    {
      return new TComponent[] {(TComponent)((object)ClassLibrary2.Class4Container.Resolve()),(TComponent)((object)ClassLibrary2.Class5Container.Resolve())};
    }
    throw new ArgumentOutOfRangeException();
  }


}