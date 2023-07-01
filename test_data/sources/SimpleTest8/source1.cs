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

[Component, Overrides<Class2>(AccessModifier.Public)]
public class Class3 : Class2
{
}

[Component, DependsOn<Class2>(AccessModifier.Private)]
public class Class4
{
}