using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class LazyInterfaceComponentInfo : ComponentInfoBase, IComponentInfo
{
  private readonly INamedTypeSymbol myInterfaceSymbol;
  private readonly ComponentInfoStorage myStorage;

  private bool myIsInitialized;
  private ComponentInfo myConcreteComponent = null!;


  public override INamedTypeSymbol ComponentSymbol
  {
    get
    {
      InitializeIfNeededOrThrow();
      return myConcreteComponent.ComponentSymbol;
    }
  }

  public override HashSet<IComponentInfo> Dependencies
  {
    get
    {
      InitializeIfNeededOrThrow();
      return myConcreteComponent.Dependencies;
    }
  }

  public override IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> OrderedDependencies
  {
    get
    {
      InitializeIfNeededOrThrow();
      return myConcreteComponent.OrderedDependencies;
    }
  }


  public LazyInterfaceComponentInfo(INamedTypeSymbol interfaceSymbol, ComponentInfoStorage storage)
  {
    myInterfaceSymbol = interfaceSymbol;
    myStorage = storage;
  }


  public override IReadOnlyList<ComponentInfo> ResolveUnderlyingConcreteComponents()
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