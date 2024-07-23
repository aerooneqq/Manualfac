using ManualfacAttributes;

namespace DefaultNamespace;


public interface ILogger
{
  
}

public interface IComponent
{
  
}

[Component, ManualInitialization, DependsOn<Private, ILogger>]
public partial class Component : IComponent
{
}