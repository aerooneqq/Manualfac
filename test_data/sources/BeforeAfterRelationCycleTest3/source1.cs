using ManualfacAttributes;
using System.Collections.Generic;

namespace DefaultNamespace;

interface IClass1
{
}

[Component, Before<Class2>]
public partial class Class1 : IClass1
{
}

[Component, Before<Class1>]
public partial class Class2 : IClass1
{
}

[Component, DependsOn<Internal, IEnumerable<IClass1>>()]
public partial class Class5
{
}