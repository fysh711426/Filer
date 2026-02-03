using System.Text.Json;
using static Filer.Extensions.PathHelper;

namespace Filer.Extensions
{
    public class DirectoryCache
    {
        public string LastWriteTimeUtc { get; set; } = "";
        public List<string> Files { get; set; } = new();
    }

    public class CachedDirectory
    {
        protected readonly string _folderPath;
        public CachedDirectory(string folderPath)
        {
            _folderPath = folderPath;
        }

        public IEnumerable<string> EnumerateCachedFiles(int workNum, string workDir)
        {
            var stack = new Stack<string>();
            stack.Push(_folderPath);
            while (stack.Count > 0)
            {
                var folderPath = stack.Pop();
                var lastWriteTimeUtc = Directory.GetLastWriteTimeUtc(folderPath)
                    .ToString("yyyy/MM/dd HH:mm:ss");

                var cacheDir = GetAppDirectory("Directory");
                var cacheSubDir = Path.GetFullPath(
                        Path.Combine(cacheDir, $"{workNum}", folderPath.Replace(workDir, "")));
                if (!cacheSubDir.StartsWith(cacheDir))
                    throw new Exception("DirectoryPath is outside of the cacheDir.");
                if (!Directory.Exists(cacheSubDir))
                    Directory.CreateDirectory(cacheSubDir);

                var cachePath = Path.Combine(cacheSubDir, "Cache.json");
                if (!File.Exists(cachePath))
                {
                    var cache = new DirectoryCache();
                    cache.LastWriteTimeUtc = lastWriteTimeUtc;

                    var files = Directory.GetFiles(folderPath);
                    cache.Files = files.ToList();

                    var json = JsonSerializer.Serialize(cache);
                    File.WriteAllText(cachePath, json);

                    foreach (var file in files)
                    {
                        yield return file;
                    }
                }
                else 
                {
                    var json = File.ReadAllText(cachePath);
                    var cache = JsonSerializer.Deserialize<DirectoryCache>(json) ?? new();

                    foreach (var file in cache.Files)
                    {
                        yield return file;
                    }
                }

                var directorys = Directory.GetDirectories(folderPath);
                foreach (var directory in directorys.Reverse())
                {
                    stack.Push(directory);
                }
            }
        }

        //public List<string> GetFiles(string directoryPath)
        //{
        //    var files = new List<string>();
        //    var stack = new Stack<string>();
        //    stack.Push(directoryPath);
        //    while (stack.Count > 0)
        //    {
        //        var item = stack.Pop();
        //        files.AddRange(Directory.GetFiles(item));
        //        var directorys = Directory.GetDirectories(item);
        //        if (directorys.Length > 0)
        //        {
        //            foreach (var directory in directorys)
        //            {
        //                stack.Push(directory);
        //            }
        //        }
        //    }
        //    return files;
        //}
    }
}