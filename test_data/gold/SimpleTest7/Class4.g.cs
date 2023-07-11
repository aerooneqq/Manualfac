using DefaultNamespace;

namespace DefaultNamespace 
{
  public partial class Class4
  {
    internal readonly DefaultNamespace.Class3 Class3;

    public Class4(
      DefaultNamespace.Class3 c0,
      DefaultNamespace.Class2 c1,
      DefaultNamespace.Class1 c2
    ) : base(
      c1,
      c2
    )
    {
      Class3 = c0;
    }

  }
}