namespace Manualfac.Generators.Components.Caches;

internal class OverridesCache
{
  private readonly Dictionary<IComponent, IComponent> myBaseToOverrides;


  public IReadOnlyDictionary<IComponent, IComponent> BaseToOverrides => myBaseToOverrides;


  public OverridesCache()
  {
    myBaseToOverrides = new Dictionary<IComponent,IComponent>();
  }


  public void AddOverride(IComponent overrideComponent, IComponent baseComponent)
  {
    if (myBaseToOverrides.ContainsKey(baseComponent)) return;

    myBaseToOverrides[baseComponent] = overrideComponent;
  }
}