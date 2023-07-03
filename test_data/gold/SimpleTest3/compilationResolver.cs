public partial class compilationResolver
{
  internal TComponent Resolve<TComponent>(

  )
  {
    if (TComponent is DefaultNamespace.Class1)
    {
      return DefaultNamespace.Class1Container.Resolve();
    }
    if (TComponent is DefaultNamespace.Class2)
    {
      return DefaultNamespace.Class2Container.Resolve();
    }
    if (TComponent is DefaultNamespace.IClass1)
    {
      return DefaultNamespace.Class1Container.Resolve();
    }

  }


}