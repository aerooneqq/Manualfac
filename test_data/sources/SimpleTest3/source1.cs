using ManualfacAttributes;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component]
public partial class Class1 : IClass1
{
}

[Component, DependsOn<Protected, IClass1>()]
public partial class Class2
{
}