using ManualfacAttributes;

namespace DefaultNamespace;

[Component]
public partial class Class1
{
}

[Component, DependsOn<Protected, Class1>()]
public partial class Class2
{
}

[Component, Overrides<Class2>()]
public partial class Class3 : Class2
{
}

[Component, DependsOn<Private, Class2>()]
public partial class Class4
{
}