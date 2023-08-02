using ManualfacAttributes;
using System.Collections.Generic;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component]
public partial class Class1 : IClass1
{
}

[Component]
public partial class Class11 : IClass1
{
}

[Component]
public partial class Class111 : IClass1
{
}

[Component]
public partial class Class1111 : IClass1
{
}

[Component, DependsOn<Internal, IEnumerable<IClass1>>()]
public partial class Class3
{
}