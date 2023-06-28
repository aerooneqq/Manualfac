namespace ManualfacAttributes;

public enum AccessModifier
{
  Public,
  Private,
  Protected,
  Internal,
  PrivateProtected,
  ProtectedInternal
}

public abstract class DependsOnAttributeBase : ManualfacAttribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T1> : DependsOnAttributeBase where T1 : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T1, T2> : DependsOnAttributeBase
  where T1 : class
  where T2 : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T1, T2, T3> : DependsOnAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T1, T2, T3, T4> : DependsOnAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T1, T2, T3, T4, T5> : DependsOnAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T1, T2, T3, T4, T5, T6> : DependsOnAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}