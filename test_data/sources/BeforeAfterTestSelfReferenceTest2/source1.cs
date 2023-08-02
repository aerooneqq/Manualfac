using ManualfacAttributes;
using System.Collections.Generic;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component, After<Class2, Class3, Class4>]
public partial class Class1 : IClass1
{
}

[Component, After<Class2, Class4>]
public partial class Class2 : IClass1
{
}

[Component, After<Class4>]
public partial class Class3 : IClass1
{
}

[Component]
public partial class Class4 : IClass1
{
}

[Component, DependsOn<Internal, IEnumerable<IClass1>>()]
public partial class Class5
{
}