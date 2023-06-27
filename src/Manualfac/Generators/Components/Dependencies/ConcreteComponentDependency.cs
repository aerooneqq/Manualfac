using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class ConcreteComponentDependency : IComponentDependency
{
  private readonly IComponentInfo myComponentInfo;

  
  public INamedTypeSymbol DependencyTypeSymbol => myComponentInfo.ComponentSymbol;

  
  public ConcreteComponentDependency(IComponentInfo componentInfo)
  {
    myComponentInfo = componentInfo;
  }
  
  
  public IReadOnlyList<IComponentInfo> ResolveUnderlyingConcreteComponents() => new[] { myComponentInfo };
}