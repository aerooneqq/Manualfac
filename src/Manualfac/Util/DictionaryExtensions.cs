namespace Manualfac.Util;

internal static class DictionaryExtensions
{
  public static void AddToList<TKey, TValue>(this IDictionary<TKey, List<TValue>> map, TKey key, TValue value)
  {
    if (map.TryGetValue(key, out var items))
    {
      items.Add(value);
    }
    else
    {
      map[key] = new List<TValue> { value };
    }
  }
}