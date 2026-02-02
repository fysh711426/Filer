using System.Collections.Concurrent;
using System.Text;
using static Filer.Extensions.PathHelper;

namespace Filer.Extensions
{
    //public class CachedChineseConverter
    //{
    //    private static readonly ConcurrentDictionary<string, string> _tradCache = new();
    //    private static readonly ConcurrentDictionary<string, string> _simpCache = new();

    //    public static string ToTraditional(string text)
    //    {
    //        if (string.IsNullOrEmpty(text))
    //            return text;

    //        return _tradCache.GetOrAdd(text, static t => ChineseConverter.ToTraditional(t));
    //    }

    //    public static string ToSimplified(string text)
    //    {
    //        if (string.IsNullOrEmpty(text))
    //            return text;

    //        return _simpCache.GetOrAdd(text, static t => ChineseConverter.ToSimplified(t));
    //    }

    //    public static void ClearCache()
    //    {
    //        _tradCache.Clear();
    //        _simpCache.Clear();
    //    }
    //}

    public class CachedChineseConverter
    {
        private static Lazy<ConcurrentDictionary<string, string>> _tradCacheLazy = 
            new(() => CreateDictionary("Traditional.cache"));

        private static Lazy<ConcurrentDictionary<string, string>> _simpCacheLazy = 
            new(() => CreateDictionary("Simplified.cache"));

        private static ConcurrentDictionary<string, string> _tradCache => _tradCacheLazy.Value;
        private static ConcurrentDictionary<string, string> _simpCache => _simpCacheLazy.Value;

        private static ConcurrentDictionary<string, string> CreateDictionary(string cacheName)
        {
            static string readField(StreamReader reader)
            {
                var sb = new StringBuilder();
                var inQuotes = false;
                
                var c = 0;
                while ((c = reader.Read()) != -1)
                {
                    var ch = (char)c;

                    if (!inQuotes)
                    {
                        if (ch == '"')
                        {
                            inQuotes = true;
                        } 
                    }
                    else
                    {
                        if (ch == '"')
                        {
                            if (reader.Peek() == '"')
                            {
                                sb.Append('"');
                                reader.Read();
                            }
                            else
                            {
                                var next = reader.Peek();
                                if (next == ',' || next == '\r' || next == '\n')
                                {
                                    reader.Read();
                                    if (next == '\r' && reader.Peek() == '\n')
                                        reader.Read();
                                }
                                break;
                            }
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                    }
                }
                return sb.ToString();
            }
            var dict = new ConcurrentDictionary<string, string>();
            try
            {
                var cacheDir = GetAppDirectory("ChineseCache");
                var cachePath = Path.Combine(cacheDir, cacheName);

                if (File.Exists(cachePath))
                {
                    using (var reader = new StreamReader(cachePath, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            var key = readField(reader);
                            var value = readField(reader);

                            if (!string.IsNullOrWhiteSpace(key) && 
                                !string.IsNullOrWhiteSpace(value))
                            {
                                dict.TryAdd(key, value);
                            }
                        }
                    }
                }
            }
            catch { throw; }
            return dict;
        }

        private static volatile int _isTradDirty = 0;
        private static volatile int _isSimpDirty = 0;

        public static string ToTraditional(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return _tradCache.GetOrAdd(text, t =>
            {
                _isTradDirty = 1;
                return ChineseConverter.ToTraditional(t);
            });
        }

        public static string ToSimplified(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return _simpCache.GetOrAdd(text, t => 
            {
                _isSimpDirty = 1;
                return ChineseConverter.ToSimplified(t);
            });
        }

        public static void SaveCache()
        {
            if (Interlocked.Exchange(ref _isTradDirty, 0) == 1)
            {
                SaveToFile("Traditional.cache", _tradCache);
            }
            if (Interlocked.Exchange(ref _isSimpDirty, 0) == 1)
            {
                SaveToFile("Simplified.cache", _simpCache);
            }
        }

        private static void SaveToFile(string cacheName, ConcurrentDictionary<string, string> dict)
        {
            static string escapeCsv(string text)
            {
                if (text == null) 
                    return @"""""";
                return $@"""{text.Replace(@"""", @"""""")}""";
            }
            try
            {
                var cacheDir = GetAppDirectory("ChineseCache");
                if (!Directory.Exists(cacheDir))
                    Directory.CreateDirectory(cacheDir);
                var cachePath = Path.Combine(cacheDir, cacheName);
                var tempPath = $"{cachePath}.temp";
                using (var writer = new StreamWriter(tempPath, false, Encoding.UTF8))
                {
                    foreach (var item in dict)
                    {
                        writer.Write(escapeCsv(item.Key));
                        writer.Write(",");
                        writer.Write(escapeCsv(item.Value));
                        writer.WriteLine();
                    }
                }
                if (File.Exists(cachePath)) 
                    File.Delete(cachePath);
                File.Move(tempPath, cachePath);
            }
            catch { throw; }
        }
    }
}