using ManualfacAttributes;

namespace DefaultNamespace;

interface IClass1
{
  
}

interface IClass11
{
  
}

[Component]
public class Class1 : IClass1, IClass11
{
}

[Component, DependsOn<ProtectedInternal, IClass1>()]
public class Class2
{
}

[Component, DependsOn<PrivateProtected, IClass11>()]
public class Class3
{
}
