﻿using ManualfacAttributes;

namespace DefaultNamespace;

[Component, DependsOn<Private, IClass5>]
public class Class1
{
}

interface IClass5
{
  
}

[Component]
public class Class5 : IClass5
{
}

[Component]
public class Class6 : IClass5
{
  
}