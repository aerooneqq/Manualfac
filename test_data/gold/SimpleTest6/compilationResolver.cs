public partial class compilationResolver
{
  internal TComponent Resolve<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(DefaultNamespace.Class1))
    {
      return (TComponent)((object)DefaultNamespace.Class1Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class11))
    {
      return (TComponent)((object)DefaultNamespace.Class11Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class111))
    {
      return (TComponent)((object)DefaultNamespace.Class111Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class1111))
    {
      return (TComponent)((object)DefaultNamespace.Class1111Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.Class3))
    {
      return (TComponent)((object)DefaultNamespace.Class3Container.Resolve());
    }
    if (typeof(TComponent) == typeof(DefaultNamespace.IClass1))
    {
      return new DefaultNamespace.IClass1[] {(TComponent)((object)DefaultNamespace.Class1Container.Resolve()),(TComponent)((object)DefaultNamespace.Class11Container.Resolve()),(TComponent)((object)DefaultNamespace.Class111Container.Resolve()),(TComponent)((object)DefaultNamespace.Class1111Container.Resolve())};
    }

  }


}