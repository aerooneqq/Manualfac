namespace Manualfac.Generators.Components.Caches;

internal class OverridesCache
{
  private readonly Dictionary<IConcreteComponent, IConcreteComponent> myBaseToOverrides;


  public IReadOnlyDictionary<IConcreteComponent, IConcreteComponent> BaseToOverrides => myBaseToOverrides;


  public OverridesCache()
  {
    myBaseToOverrides = new Dictionary<IConcreteComponent,IConcreteComponent>();
  }


  public void AddOverride(IConcreteComponent overrideComponent, IConcreteComponent baseComponent)
  {
    if (myBaseToOverrides.ContainsKey(baseComponent)) return;

    myBaseToOverrides[baseComponent] = overrideComponent;
  }
}