namespace ManualfacAttributes;

public class AsAttributeBase : Attribute
{
}

public class AsAttribute<TInterface> : AsAttributeBase where TInterface : class
{
}

public class AsAttribute<T1, T2> : AsAttributeBase
  where T1 : class
  where T2 : class
{
}

public class AsAttribute<T1, T2, T3> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
{
}

public class AsAttribute<T1, T2, T3, T4> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
{
}

public class AsAttribute<T1, T2, T3, T4, T5> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
{
}

public class AsAttribute<T1, T2, T3, T4, T5, T6> : AsAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class
{
}