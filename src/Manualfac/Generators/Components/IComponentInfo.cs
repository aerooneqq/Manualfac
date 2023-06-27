using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal interface IComponentInfo
{
  INamedTypeSymbol ComponentSymbol { get; }
  HashSet<IComponentDependency> Dependencies { get; }
  IReadOnlyList<(IComponentDependency Component, AccessModifier Modifier)> OrderedDependencies { get; }
  
  string TypeShortName { get; }
  string FullName { get; }
  string? Namespace { get; }

  IReadOnlyList<IComponentInfo> ResolveConcreteDependencies();
}

internal interface IComponentDependency
{
  INamedTypeSymbol DependencyTypeSymbol { get; }
  
  IReadOnlyList<IComponentInfo> ResolveUnderlyingConcreteComponents();
}

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

internal class LazyNonCollectionInterfaceDependency : IComponentDependency
{
  private readonly INamedTypeSymbol myInterfaceSymbol;
  private readonly ComponentInfoStorage myStorage;


  private bool myIsInitialized;
  private IComponentInfo myConcreteComponent = null!;


  public INamedTypeSymbol DependencyTypeSymbol => myInterfaceSymbol;


  public LazyNonCollectionInterfaceDependency(INamedTypeSymbol interfaceSymbol, ComponentInfoStorage storage)
  {
    myInterfaceSymbol = interfaceSymbol;
    myStorage = storage;
  }
  

  public IReadOnlyList<IComponentInfo> ResolveUnderlyingConcreteComponents()
  {
    InitializeIfNeededOrThrow();
    return new[] { myConcreteComponent };
  }

  private void InitializeIfNeededOrThrow()
  {
    if (myIsInitialized) return;

    if (!myStorage.InterfacesToComponents.TryGetValue(myInterfaceSymbol, out var impls))
    {
      throw new NoImplementationForInterfaceException(myInterfaceSymbol);
    }

    if (impls.Count != 1)
    {
      throw new CantResolveConcreteImplementationException(myInterfaceSymbol, impls);
    }

    myConcreteComponent = (ComponentInfo)impls[0];
    myIsInitialized = true;
  }
}