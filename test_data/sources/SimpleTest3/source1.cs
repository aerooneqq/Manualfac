using ManualfacAttributes;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component]
public class Class1 : IClass1
{
}

[Component, DependsOn<IClass1>(AccessModifier.Protected)]
public class Class2
{
}