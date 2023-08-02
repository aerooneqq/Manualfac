using ManualfacAttributes;

namespace DefaultNamespace;

public class MyComponentAttribute : ComponentAttribute
{
}

interface IClass1
{
  
}

[MyComponent, Before<Class2, Class3, Class4>]
public class Class1 : IClass1
{
}

[MyComponent, Before<Class3, Class4>]
public class Class2 : IClass1
{
}

[MyComponent, Before<Class4>]
public class Class3 : IClass1
{
}

[MyComponent]
public class Class4 : IClass1
{
}

[MyComponent, DependsOn<Internal, IEnumerable<IClass1>>()]
public class Class5
{
}