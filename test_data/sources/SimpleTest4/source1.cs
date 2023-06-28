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

[Component, DependsOn<IClass1>(AccessModifier.ProtectedInternal)]
public class Class2
{
}

[Component, DependsOn<IClass11>(AccessModifier.PrivateProtected)]
public class Class3
{
}
