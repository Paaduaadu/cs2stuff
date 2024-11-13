public static class EnumerableExtensions {
    public static ValueTask<Dictionary<TKey, TValue>> ToDictionaryAsync<TKey, TValue>(this IAsyncEnumerable<TValue> e, Func<TValue, TKey> getKey, Func<IEnumerable<TValue>, TValue> aggregateDuplicates) where TKey: notnull{
        return e
            .GroupBy(getKey)
            .ToDictionaryAsync(
                rec => rec.Key!,
                rec => aggregateDuplicates(rec.ToEnumerable()));
    }
}