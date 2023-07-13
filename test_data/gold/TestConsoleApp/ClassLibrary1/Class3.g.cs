using ClassLibrary2;

namespace ClassLibrary1 
{
  public partial class Class3
  {
    private readonly ClassLibrary2.Class4 prefixClass4suffix;
    private readonly ClassLibrary2.Class5 prefixClass5suffix;

    public Class3(
      ClassLibrary2.Class4 c0,
      ClassLibrary2.Class5 c1
    )
    {
      prefixClass4suffix = c0;
      prefixClass5suffix = c1;
    }

  }
}