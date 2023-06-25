using ClassLibrary2;
using ManualfacAttributes;

namespace ClassLibrary1;

[Component]
[DependsOn<Class4, Class5>(AccessModifier.Private)]
public partial class Class3
{
}