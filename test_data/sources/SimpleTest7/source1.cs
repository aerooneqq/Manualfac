using ManualfacAttributes;

namespace DefaultNamespace;

[Component]
public partial class Class1
{
}

[Component, DependsOn<Protected, Class1>()]
public partial class Class2 : Class1
{
}

[Component, DependsOn<Public, Class2>()]
public partial class Class3 : Class2
{
}

[Component, DependsOn<Internal, Class3>()]
public partial class Class4 : Class3
{
}

[Component, DependsOn<PrivateProtected, Class4>()]
public partial class Class5 : Class4
{
}