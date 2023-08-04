namespace Manualfac.Components.Caches;

internal class OverridesCache
{
  private readonly Dictionary<IComponent, IComponent> myBaseToOverrides = new();


  public IReadOnlyDictionary<IComponent, IComponent> BaseToOverrides => myBaseToOverrides;


  public void AddOverride(IComponent overrideComponent, IComponent baseComponent)
  {
    if (myBaseToOverrides.ContainsKey(baseComponent)) return;

    myBaseToOverrides[baseComponent] = overrideComponent;
  }
}