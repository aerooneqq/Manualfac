using ManualfacAttributes;

namespace DefaultNamespace;

[Component]
public class Class1
{
}

[Component, DependsOn<Protected, Class1>()]
public class Class2
{
}

[Component, DependsOn<Public, Class1, Class1>()]
public class Class3
{
}

[Component, DependsOn<Internal, Class1, Class2, Class3>()]
public class Class4
{
}

[Component, DependsOn<PrivateProtected, Class1, Class2, Class3, Class4>()]
public class Class5
{
}