using ManualfacAttributes;

namespace DefaultNamespace;

interface IClass1
{
  
}

interface IClass11
{
  
}

[Component, As<IClass11>]
public partial class Class1 : IClass1, IClass11
{
}

[Component, DependsOn<PrivateProtected, IClass11>()]
public partial class Class3
{
}