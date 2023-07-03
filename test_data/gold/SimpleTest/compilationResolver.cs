public partial class compilationResolver
{
  internal static TComponent ResolveOrThrow<TComponent>(

  )
  {
    if (typeof(TComponent) == typeof(asd.Class1))
    {
      return (TComponent)((object)asd.Class1Container.Resolve());
    }
    throw new ArgumentOutOfRangeException();
  }

  internal static IEnumerable<TComponent> ResolveComponentsOrThrow<TComponent>(

  )
  {
    throw new ArgumentOutOfRangeException();
  }


}