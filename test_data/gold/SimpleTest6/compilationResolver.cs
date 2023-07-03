public partial class compilationResolver
{
  internal TComponent Resolve<TComponent>(

  )
  {
    if (TComponent is DefaultNamespace.Class1)
    {
      return DefaultNamespace.Class1Container.Resolve();
    }
    if (TComponent is DefaultNamespace.Class11)
    {
      return DefaultNamespace.Class11Container.Resolve();
    }
    if (TComponent is DefaultNamespace.Class111)
    {
      return DefaultNamespace.Class111Container.Resolve();
    }
    if (TComponent is DefaultNamespace.Class1111)
    {
      return DefaultNamespace.Class1111Container.Resolve();
    }
    if (TComponent is DefaultNamespace.Class3)
    {
      return DefaultNamespace.Class3Container.Resolve();
    }
    if (TComponent is IEnumerable<DefaultNamespace.IClass1>)
    {
      return new DefaultNamespace.IClass1[] {DefaultNamespace.Class1Container.Resolve(),DefaultNamespace.Class11Container.Resolve(),DefaultNamespace.Class111Container.Resolve(),DefaultNamespace.Class1111Container.Resolve()};
    }

  }


}