namespace publishmetrics.Core
{
    public static class FunctionalExtensions
    {
        public static Func<string, T> WithCache<T>(Func<string, T> value, Dictionary<string, T> cache) =>
            key =>
                {
                    if (!cache.TryGetValue(key, out T? result))
                    {
                        result = value(key);
                        cache.Add(key, result);
                    }
                    return result;
                };
                
        public static async Task<IAsyncEnumerable<T>> AppendIf<T>(this IAsyncEnumerable<T> e, T newElement, Func<IAsyncEnumerable<T>, Task<bool>> predicate) =>
            await predicate(e) ? e.Append(newElement) : e;

    }
}