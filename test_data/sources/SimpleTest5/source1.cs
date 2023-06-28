using ManualfacAttributes;

namespace DefaultNamespace;

interface IClass1
{
  
}

interface IClass11
{
  
}

[Component, As<IClass11>]
public class Class1 : IClass1, IClass11
{
}

[Component, DependsOn<IClass11>(AccessModifier.PrivateProtected)]
public class Class3
{
}