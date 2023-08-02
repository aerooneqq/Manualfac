using ManualfacAttributes;
using System.Collections.Generic;

namespace DefaultNamespace;

interface IClass1
{
  
}

[Component, After<Class2, Class3, Class4>]
public class Class1 : IClass1
{
}

[Component, After<Class3, Class4>]
public class Class2 : IClass1
{
}

[Component, After<Class4>]
public class Class3 : IClass1
{
}

[Component, After<Class1>]
public class Class4 : IClass1
{
}

[Component, DependsOn<Internal, IEnumerable<IClass1>>()]
public class Class5
{
}