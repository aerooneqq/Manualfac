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

[Component, DependsOn<Public, Class1, Class2>()]
public partial class Class3
{
}

[Component, DependsOn<Internal, Class1, Class2, Class3>()]
public partial class Class4
{
}

[Component, DependsOn<PrivateProtected, Class1, Class2, Class3, Class4>()]
public partial class Class5
{
}