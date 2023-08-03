namespace ManualfacAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class DependsOnAttributeBase : ManualfacAttribute;

public sealed class DependsOnAttribute<TAccess, T1> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class;

public sealed class DependsOnAttribute<TAccess, T1, T2> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class;

public sealed class DependsOnAttribute<TAccess, T1, T2, T3> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class;

public sealed class DependsOnAttribute<TAccess, T1, T2, T3, T4> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class;

public sealed class DependsOnAttribute<TAccess, T1, T2, T3, T4, T5> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class;

public sealed class DependsOnAttribute<TAccess, T1, T2, T3, T4, T5, T6> : DependsOnAttributeBase
  where TAccess : AccessModifier
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class;