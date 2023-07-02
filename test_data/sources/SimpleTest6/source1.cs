using ManualfacAttributes;
using System.Collections.Generic;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component]
public class Class1 : IClass1
{
}

[Component]
public class Class11 : IClass1
{
}

[Component]
public class Class111 : IClass1
{
}

[Component]
public class Class1111 : IClass1
{
}

[Component, DependsOn<Internal, IEnumerable<IClass1>>()]
public class Class3
{
}