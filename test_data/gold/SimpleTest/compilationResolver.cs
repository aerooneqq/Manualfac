public partial class compilationResolver
{
  internal TComponent Resolve<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(asd.Class1))
    {
      return (TComponent)((object)asd.Class1Container.Resolve());
    }

  }


}