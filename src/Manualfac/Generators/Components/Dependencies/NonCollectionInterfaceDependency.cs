using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class NonCollectionInterfaceDependency : IComponentDependency
{
  private readonly ComponentsStorage myStorage;


  private bool myIsInitialized;
  private IReadOnlyList<IConcreteComponent> myResolveResult = null!;


  public INamedTypeSymbol DependencyTypeSymbol { get; }


  public NonCollectionInterfaceDependency(INamedTypeSymbol interfaceSymbol, ComponentsStorage storage)
  {
    DependencyTypeSymbol = interfaceSymbol;
    myStorage = storage;
  }
  

  public IReadOnlyList<IConcreteComponent> ResolveUnderlyingConcreteComponents()
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
      throw new CantResolveConcreteImplementationException(DependencyTypeSymbol, impls.Select(impl => impl.FullName).ToList());
    }

    myResolveResult = new[] { impls[0] };
    myIsInitialized = true;
  }
}