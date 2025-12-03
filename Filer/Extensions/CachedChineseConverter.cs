using System.Collections.Concurrent;

namespace Filer.Extensions
{
    public class CachedChineseConverter
    {
        private static readonly ConcurrentDictionary<string, string> _tradCache = new();
        private static readonly ConcurrentDictionary<string, string> _simpCache = new();

        public static string ToTraditional(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return _tradCache.GetOrAdd(text, static t => ChineseConverter.ToTraditional(t));
        }

        public static string ToSimplified(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return _simpCache.GetOrAdd(text, static t => ChineseConverter.ToSimplified(t));
        }

        public static void ClearCache()
        {
            _tradCache.Clear();
            _simpCache.Clear();
        }
    }
}