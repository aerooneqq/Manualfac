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
    if (typeof(TComponent) == typeof(DefaultNamespace.IClass1))
    {
      return (TComponent)((object)DefaultNamespace.Class1Container.Resolve());
    }

  }


}