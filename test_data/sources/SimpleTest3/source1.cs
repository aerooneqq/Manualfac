using ManualfacAttributes;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component]
public class Class1 : IClass1
{
}

[Component, DependsOn<Protected, IClass1>()]
public class Class2
{
}