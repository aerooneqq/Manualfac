using ManualfacAttributes;

namespace DefaultNamespace;

[Component, DependsOn<Private, IClass5>]
public partial class Class1
{
}

interface IClass5
{
  
}

[Component]
public partial class Class5 : IClass5
{
}

[Component]
public partial class Class6 : IClass5
{
  
}