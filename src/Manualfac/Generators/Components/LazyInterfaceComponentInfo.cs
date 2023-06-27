using Manualfac.Exceptions;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Components;

internal class LazyInterfaceComponentInfo : ComponentInfoBase, IComponentInfo
{
  private readonly INamedTypeSymbol myInterfaceSymbol;
  private readonly ComponentInfoStorage myStorage;

  private bool myIsInitialized;
  private INamedTypeSymbol myComponentSymbol = null!;
  private HashSet<IComponentInfo> myDependencies = null!;
  private IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> myOrderedDependencies = null!;

  
  public override INamedTypeSymbol ComponentSymbol
  {
    get
    {
      InitializeIfNeededOrThrow();
      return myComponentSymbol;
    }
  }

  public override HashSet<IComponentInfo> Dependencies
  {
    get
    {
      InitializeIfNeededOrThrow();
      return myDependencies;
    }
  }

  public override IReadOnlyList<(IComponentInfo Component, AccessModifier Modifier)> OrderedDependencies
  {
    get
    {
      InitializeIfNeededOrThrow();
      return myOrderedDependencies;
    }
  }


  public LazyInterfaceComponentInfo(INamedTypeSymbol interfaceSymbol, ComponentInfoStorage storage)
  {
    myInterfaceSymbol = interfaceSymbol;
    myStorage = storage;
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

    var resolvedImpl = impls[0];
    myComponentSymbol = resolvedImpl.ComponentSymbol;
    myDependencies = new HashSet<IComponentInfo>(resolvedImpl.Dependencies);
    myOrderedDependencies = resolvedImpl.OrderedDependencies.ToArray();
    
    myIsInitialized = true;
  }
}