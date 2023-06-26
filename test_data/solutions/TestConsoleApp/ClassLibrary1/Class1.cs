using ManualfacAttributes;

namespace ClassLibrary1;

[Component]
[DependsOn<Class2, Class3>(AccessModifier.Private)]
public partial class Class1
{
}