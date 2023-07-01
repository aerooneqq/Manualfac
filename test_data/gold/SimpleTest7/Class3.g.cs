using DefaultNamespace;

namespace DefaultNamespace 
{
  public partial class Class3
  {
    public readonly DefaultNamespace.Class2 myClass2;

    public Class3(
      DefaultNamespace.Class2 c0,
      DefaultNamespace.Class1 c1
    ) : base(
      c1
    )
    {
      myClass2 = c0;
    }

  }
}