public partial class TestConsoleAppInitializer
{
  internal static void Initialize(

  )
  {
    ClassLibrary2.Class4Container.Initialize(
      () => 
      {
        var created = new ClassLibrary2.Class4(

        );
        return created;
      }
    );

    ClassLibrary2.Class5Container.Initialize(
      () => 
      {
        var created = new ClassLibrary2.Class5(
          ClassLibrary2.Class4Container.Resolve()
        );
        return created;
      }
    );

    ClassLibrary1.Class3Container.Initialize(
      () => 
      {
        return Class6Container.Resolve();
      }
    );

    Class6Container.Initialize(
      () => 
      {
        var created = new Class6(
          ClassLibrary2.Class4Container.Resolve(),
          ClassLibrary2.Class4Container.Resolve(),
          ClassLibrary2.Class5Container.Resolve()
        );
        return created;
      }
    );

    ClassLibrary1.Class2Container.Initialize(
      () => 
      {
        var created = new ClassLibrary1.Class2(
          Class6Container.Resolve()
        );
        return created;
      }
    );

    ClassLibrary1.Class1Container.Initialize(
      () => 
      {
        var created = new ClassLibrary1.Class1(
          ClassLibrary1.Class2Container.Resolve(),
          Class6Container.Resolve(),
          ClassLibrary2.Class4Container.Resolve()
        );
        return created;
      }
    );


  }


}