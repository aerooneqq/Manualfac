using DefaultNamespace;

namespace DefaultNamespace 
{
  public partial class Class4
  {
    internal readonly DefaultNamespace.Class3 myClass3;

    public Class4(
      DefaultNamespace.Class3 c0,
      DefaultNamespace.Class2 c1,
      DefaultNamespace.Class1 c2
    ) : base(
      c1,
      c2
    )
    {
      myClass3 = c0;
    }

  }
}