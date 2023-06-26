using ManualfacAttributes;

namespace DefaultNamespace;

[Component]
public class Class1
{
}

[Component, DependsOn<Class1>(AccessModifier.Protected)]
public class Class2
{
}

[Component, DependsOn<Class1, Class2>(AccessModifier.Public)]
public class Class3
{
}

[Component, DependsOn<Class1, Class2, Class3>(AccessModifier.Internal)]
public class Class4
{
}

[Component, DependsOn<Class1, Class2, Class3, Class4>(AccessModifier.PrivateProtected)]
public class Class5
{
}