using ClassLibrary2;
using ManualfacAttributes;

namespace ClassLibrary1;

[Component]
[DependsOn<Class2, Class3>]
public partial class Class1
{
}