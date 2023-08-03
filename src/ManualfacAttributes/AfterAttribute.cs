namespace ManualfacAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class AfterAttributeBase : ManualfacAttribute;

public class AfterAttribute<T> : AfterAttributeBase where T : class;

public class AfterAttribute<T1, T2> : AfterAttributeBase
  where T1 : class
  where T2 : class;

public class AfterAttribute<T1, T2, T3> : AfterAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class;

public class AfterAttribute<T1, T2, T3, T4> : AfterAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class;

public class AfterAttribute<T1, T2, T3, T4, T5> : AfterAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class;

public class AfterAttribute<T1, T2, T3, T4, T5, T6> : AfterAttributeBase
  where T1 : class
  where T2 : class
  where T3 : class
  where T4 : class
  where T5 : class
  where T6 : class;