namespace publishmetrics.Core
{
    public static class FunctionalExtensions
    {
        public static Func<string, T> Memoize<T>(Func<string, T> value, Dictionary<string, T> cache) =>
            key =>
                {
                    if (!cache.TryGetValue(key, out T? result))
                    {
                        result = value(key);
                        cache.Add(key, result);
                    }
                    return result;
                };
    }
}