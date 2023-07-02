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

[Component, Overrides<Class2>()]
public class Class3 : Class2
{
}

[Component, DependsOn<Private, Class2>()]
public class Class4
{
}