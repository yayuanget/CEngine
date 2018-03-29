using System;
using System.Text;

public static class StringBuilderCache
{
    [ThreadStatic]
    private static StringBuilder _cache = new StringBuilder();
    private const int MAX_BUILDER_SIZE = 512;
    public static StringBuilder Acquire(int capacity = 256)
    {
        StringBuilder cache = _cache;
        if (cache != null && cache.Capacity >= capacity)
        {
            _cache = null;
            Clear(cache);
            return cache;
        }
        return new StringBuilder(capacity);
    }
    public static string GetStringAndRelease(StringBuilder sb)
    {
        string arg_0C_0 = sb.ToString();
        Release(sb);
        return arg_0C_0;
    }
    public static void Release(StringBuilder sb)
    {
        if (sb.Capacity <= 512)
        {
            _cache = sb;
        }
    }

    public static void Clear(StringBuilder sb)
    {
        sb.Length = 0;
    }
}
