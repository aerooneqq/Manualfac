namespace ManualfacAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class BeforeAttributeBase : ManualfacAttribute
{
}

public class BeforeAttribute<T> : BeforeAttributeBase where T : class
{
}

public class BeforeAttribute<T1, T2> : BeforeAttributeBase 
  where T1 : class 
  where T2 : class
{
}

public class BeforeAttribute<T1, T2, T3> : BeforeAttributeBase 
  where T1 : class 
  where T2 : class 
  where T3 : class
{
}

public class BeforeAttribute<T1, T2, T3, T4> : BeforeAttributeBase
  where T1 : class 
  where T2 : class 
  where T3 : class 
  where T4 : class
{
}

public class BeforeAttribute<T1, T2, T3, T4, T5> : BeforeAttributeBase
  where T1 : class 
  where T2 : class 
  where T3 : class 
  where T4 : class
  where T5 : class
{
}

public class BeforeAttribute<T1, T2, T3, T4, T5, T6> : BeforeAttributeBase
  where T1 : class 
  where T2 : class 
  where T3 : class 
  where T4 : class
  where T5 : class
  where T6 : class
{
}
