public partial class compilationResolver
{
  internal TComponent Resolve<TComponent>(

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

  }


}