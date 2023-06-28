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

[Component, DependsOn<IEnumerable<IClass1>>(AccessModifier.Internal)]
public class Class3
{
}