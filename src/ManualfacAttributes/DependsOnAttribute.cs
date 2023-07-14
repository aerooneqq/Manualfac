namespace ManualfacAttributes;


public class AccessModifier {}
public sealed class Private : AccessModifier {}
public sealed class Public : AccessModifier {}
public sealed class Internal : AccessModifier {}
public sealed class PrivateProtected : AccessModifier {}
public sealed class ProtectedInternal : AccessModifier {}


public abstract class DependsOnAttributeBase : ManualfacAttribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TAccess, T1> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TAccess, T1, T2> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TAccess, T1, T2, T3> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TAccess, T1, T2, T3, T4> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TAccess, T1, T2, T3, T4, T5> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TAccess, T1, T2, T3, T4, T5, T6> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class
{
}