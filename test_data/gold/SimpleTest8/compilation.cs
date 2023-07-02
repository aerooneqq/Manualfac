public partial class compilationInitializer
{
  internal static void Initialize(

  )
  {
    DefaultNamespace.Class1Container.Initialize(
      () => 
      {
        var created = new DefaultNamespace.Class1(

        );
        return created;
      }
    );

    DefaultNamespace.Class2Container.Initialize(
      () => 
      {
        var created = new DefaultNamespace.Class2(
          Class1Container.Resolve()
        );
        return created;
      }
    );

    DefaultNamespace.Class3Container.Initialize(
      () => 
      {
        var created = new DefaultNamespace.Class3(
          Class1Container.Resolve()
        );
        return created;
      }
    );

    DefaultNamespace.Class4Container.Initialize(
      () => 
      {
        var created = new DefaultNamespace.Class4(
          Class3Container.Resolve()
        );
        return created;
      }
    );


  }


}