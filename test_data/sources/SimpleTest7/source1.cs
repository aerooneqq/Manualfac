using ManualfacAttributes;

namespace DefaultNamespace;

[Component]
public class Class1
{
}

[Component, DependsOn<Protected, Class1>()]
public class Class2 : Class1
{
}

[Component, DependsOn<Public, Class2>()]
public class Class3 : Class2
{
}

[Component, DependsOn<Internal, Class3>()]
public class Class4 : Class3
{
}

[Component, DependsOn<PrivateProtected, Class4>()]
public class Class5 : Class4
{
}