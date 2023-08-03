using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class NonCollectionInterfaceDependency : IComponentDependency
{
  private readonly ComponentsStorage myStorage;
  private readonly Lazy<IReadOnlyList<IComponent>> myResolveResult;


  public INamedTypeSymbol DependencyTypeSymbol { get; }


  public NonCollectionInterfaceDependency(INamedTypeSymbol interfaceSymbol, ComponentsStorage storage)
  {
    DependencyTypeSymbol = interfaceSymbol;
    myStorage = storage;
    myResolveResult = new Lazy<IReadOnlyList<IComponent>>(InitializeOrThrow);
  }


  public IReadOnlyList<IComponent> ResolveUnderlyingConcreteComponents()
  {
    return myResolveResult.Value;
  }

  private IReadOnlyList<IComponent> InitializeOrThrow()
  {
    if (!myStorage.InterfacesToComponents.TryGetValue(DependencyTypeSymbol, out var impls))
    {
      throw new NoImplementationForInterfaceException(DependencyTypeSymbol);
    }

    if (impls.Count != 1)
    {
      throw new CantResolveConcreteImplementationException(
        DependencyTypeSymbol, impls.Select(impl => impl.FullName).ToList());
    }

    return new[] { impls[0] };
  }
}