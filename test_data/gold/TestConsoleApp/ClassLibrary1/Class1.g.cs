using ClassLibrary1;
using ClassLibrary2;

namespace ClassLibrary1 
{
  public partial class Class1
  {
    private readonly ClassLibrary1.Class2 asdadClass2;
    private readonly ClassLibrary1.Class3 asdadClass3;

    public Class1(
      ClassLibrary1.Class2 c0,
      ClassLibrary1.Class3 c1,
      ClassLibrary2.Class4 c2
    ) : base(
      c2
    )
    {
      asdadClass2 = c0;
      asdadClass3 = c1;
    }

  }
}