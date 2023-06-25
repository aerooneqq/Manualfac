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
public class DependsOnAttribute<TF> : DependsOnAttributeBase where TF : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS> : DependsOnAttributeBase
  where TF : class
  where TS : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT> : DependsOnAttributeBase
  where TF : class
  where TS : class
  where TT : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT, TFourth> : DependsOnAttributeBase
  where TF : class
  where TS : class
  where TT : class
  where TFourth : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT, TFourth, TFifth> : DependsOnAttributeBase
  where TF : class
  where TS : class
  where TT : class
  where TFourth : class
  where TFifth : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT, TFourth, TFifth, TSixth> : DependsOnAttributeBase
  where TF : class
  where TS : class
  where TT : class
  where TFourth : class
  where TFifth : class
  where TSixth : class
{
  public DependsOnAttribute(AccessModifier modifier)
  {
  }
}