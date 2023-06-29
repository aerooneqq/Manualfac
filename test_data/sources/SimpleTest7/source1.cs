using ManualfacAttributes;

namespace DefaultNamespace;

[Component]
public class Class1
{
}

[Component, DependsOn<Class1>(AccessModifier.Protected)]
public class Class2 : Class1
{
}

[Component, DependsOn<Class2>(AccessModifier.Public)]
public class Class3 : Class2
{
}

[Component, DependsOn<Class3>(AccessModifier.Internal)]
public class Class4 : Class3
{
}

[Component, DependsOn<Class4>(AccessModifier.PrivateProtected)]
public class Class5 : Class4
{
}