public partial class compilationResolver
{
  internal static TComponent ResolveOrThrow<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(DefaultNamespace.Class1))
    {
      return (TComponent)((object)DefaultNamespace.Class1Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class2))
    {
      return (TComponent)((object)DefaultNamespace.Class2Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class3))
    {
      return (TComponent)((object)DefaultNamespace.Class3Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class4))
    {
      return (TComponent)((object)DefaultNamespace.Class4Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class5))
    {
      return (TComponent)((object)DefaultNamespace.Class5Container.Resolve());
    }
    throw new ArgumentOutOfRangeException();
  }

  internal static IEnumerable<TComponent> ResolveComponentsOrThrow<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(DefaultNamespace.IClass1))
    {
      return new TComponent[] {(TComponent)((object)DefaultNamespace.Class1Container.Resolve()),(TComponent)((object)DefaultNamespace.Class2Container.Resolve()),(TComponent)((object)DefaultNamespace.Class3Container.Resolve()),(TComponent)((object)DefaultNamespace.Class4Container.Resolve())};
    }
    throw new ArgumentOutOfRangeException();
  }


}