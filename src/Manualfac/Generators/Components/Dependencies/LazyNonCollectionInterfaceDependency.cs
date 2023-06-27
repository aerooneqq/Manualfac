using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class LazyNonCollectionInterfaceDependency : IComponentDependency
{
  private readonly ComponentInfoStorage myStorage;


  private bool myIsInitialized;
  private IReadOnlyList<IComponentInfo> myResolveResult = null!;


  public INamedTypeSymbol DependencyTypeSymbol { get; }


  public LazyNonCollectionInterfaceDependency(INamedTypeSymbol interfaceSymbol, ComponentInfoStorage storage)
  {
    DependencyTypeSymbol = interfaceSymbol;
    myStorage = storage;
  }
  

  public IReadOnlyList<IComponentInfo> ResolveUnderlyingConcreteComponents()
  {
    InitializeIfNeededOrThrow();
    return myResolveResult;
  }

  private void InitializeIfNeededOrThrow()
  {
    if (myIsInitialized) return;

    if (!myStorage.InterfacesToComponents.TryGetValue(DependencyTypeSymbol, out var impls))
    {
      throw new NoImplementationForInterfaceException(DependencyTypeSymbol);
    }

    if (impls.Count != 1)
    {
      throw new CantResolveConcreteImplementationException(DependencyTypeSymbol, impls);
    }

    myResolveResult = new[] { impls[0] };
    myIsInitialized = true;
  }
}