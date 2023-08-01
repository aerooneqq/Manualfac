namespace ManualfacAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class AsAttributeBase : Attribute
{
}

public sealed class AsAttribute<TInterface> : AsAttributeBase where TInterface : class
{
}

public sealed class AsAttribute<T1, T2> : AsAttributeBase
  where T1 : class
  where T2 : class
{
}

public sealed class AsAttribute<T1, T2, T3> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
{
}

public sealed class AsAttribute<T1, T2, T3, T4> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
{
}

public sealed class AsAttribute<T1, T2, T3, T4, T5> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
{
}

public sealed class AsAttribute<T1, T2, T3, T4, T5, T6> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class
{
}