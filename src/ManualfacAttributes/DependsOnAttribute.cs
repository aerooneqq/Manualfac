namespace ManualfacAttributes;


public class AccessModifier {}
public class Private : AccessModifier {}
public class Public : AccessModifier {}
public class Internal : AccessModifier {}
public class PrivateProtected : AccessModifier {}
public class ProtectedInternal : AccessModifier {}


public abstract class DependsOnAttributeBase : ManualfacAttribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TAccess, T1> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TAccess, T1, T2> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TAccess, T1, T2, T3> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TAccess, T1, T2, T3, T4> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TAccess, T1, T2, T3, T4, T5> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TAccess, T1, T2, T3, T4, T5, T6> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class
{
}