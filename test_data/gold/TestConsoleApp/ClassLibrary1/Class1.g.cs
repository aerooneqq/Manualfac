using ClassLibrary1;
using ClassLibrary2;

namespace ClassLibrary1 
{
  public partial class Class1
  {
    private readonly ClassLibrary1.Class2 prefixClass2suffix;
    private readonly ClassLibrary1.Class3 prefixClass3suffix;

    public Class1(
      ClassLibrary1.Class2 c0,
      ClassLibrary1.Class3 c1,
      ClassLibrary2.Class4 c2
    ) : base(
      c2
    )
    {
      prefixClass2suffix = c0;
      prefixClass3suffix = c1;
    }

  }
}