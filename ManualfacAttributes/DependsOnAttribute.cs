namespace ManualfacAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF> : ManualfacAttribute where TF : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS> : ManualfacAttribute
  where TF : class
  where TS : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT> : ManualfacAttribute
  where TF : class
  where TS : class
  where TT : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT, TFourth> : ManualfacAttribute
  where TF : class
  where TS : class
  where TT : class
  where TFourth : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT, TFourth, TFifth> : ManualfacAttribute
  where TF : class
  where TS : class
  where TT : class
  where TFourth : class
  where TFifth : class
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<TF, TS, TT, TFourth, TFifth, TSixth> : ManualfacAttribute
  where TF : class
  where TS : class
  where TT : class
  where TFourth : class
  where TFifth : class
  where TSixth : class
{
}