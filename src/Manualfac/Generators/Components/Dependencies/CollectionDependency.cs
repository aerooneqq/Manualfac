using System.Diagnostics;
using Manualfac.Exceptions;
using Manualfac.Generators.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components.Dependencies;

internal class CollectionDependency : IComponentDependency
{
  private readonly ComponentsStorage myStorage;


  public INamedTypeSymbol DependencyTypeSymbol { get; }
  public INamedTypeSymbol CollectionItemInterface { get; }


  public CollectionDependency(INamedTypeSymbol collectionInterface, ComponentsStorage storage)
  {
    myStorage = storage;
    DependencyTypeSymbol = collectionInterface;

    Debug.Assert(collectionInterface.TypeArguments.Length == 1);
    Debug.Assert(collectionInterface.MetadataName == Constants.GenericIEnumerable);

    CollectionItemInterface = (INamedTypeSymbol)collectionInterface.TypeArguments.First();

    Debug.Assert(CollectionItemInterface.TypeKind == TypeKind.Interface);
  }


  public IReadOnlyList<IComponent> ResolveUnderlyingConcreteComponents()
  {
    if (!myStorage.InterfacesToComponents.TryGetValue(CollectionItemInterface, out var components))
    {
      throw new NoImplementationForInterfaceException(CollectionItemInterface);
    }

    return components;
  }
}