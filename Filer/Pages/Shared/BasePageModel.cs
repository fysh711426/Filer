using Filer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Filer.Pages.Shared
{
    public class BasePageModel : PageModel
    {
        public static readonly string v = "1.0.0-alpha2";

        public string? Data { get; set; } = null;

        protected static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };
        
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IConfiguration _configuration;
        protected readonly List<WorkDir> _workDirs;
        public BasePageModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            var workDirs = _configuration
                .GetSection("WorkDirs").Get<List<WorkDir>>()
                ?? new List<WorkDir>();
            var index = 0;
            foreach(var item in workDirs)
            {
                item.Path = item.Path.TrimEnd(Path.DirectorySeparatorChar);
                item.Path = $@"{item.Path}{Path.DirectorySeparatorChar}";
                item.IsPathError = !Directory.Exists(item.Path);
                item.Index = index++;
            }
            _workDirs = workDirs;
        }

        protected (string parentPath, string parentName, string path, string pathName) 
            GetPathInfo(int workNum, string path)
        {
            var parentPath = Path
                .GetDirectoryName(path)?.Replace(@"\", "/") ?? "";
            return (
                parentPath, GetPathName(workNum, parentPath),
                path, GetPathName(workNum, path)
            );
        }

        private string GetPathName(int workNum, string path)
        {
            if (path == "")
                return _workDirs[workNum - 1].Name;
            return Path.GetFileName(path);
        }

        protected readonly Dictionary<string, bool> _imageMimeType = new()
        {
            ["image/jpeg"] = true,
            ["image/png"] = true,
            ["image/gif"] = true,
            ["image/webp"] = true
        };

        protected readonly Dictionary<string, bool> _videoMimeType = new()
        {
            ["video/mp4"] = true
        };

        protected readonly Dictionary<string, bool> _audioMimeType = new()
        {
            ["audio/mpeg"] = true
        };

        protected readonly Dictionary<string, bool> _textMimeType = new()
        {
            ["text/plain"] = true
        };

        protected static string FormatFileSize(double fileSize)
        {
            if (fileSize < 0)
            {
                return "Error";
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                var size = fileSize / (1024 * 1024 * 1024);
                return string.Format("{0:########0.00} GB",
                    Math.Floor(size * 100) / 100);
            }
            else if (fileSize >= 1024 * 1024)
            {
                var size = fileSize / (1024 * 1024);
                return string.Format("{0:####0.00} MB",
                    Math.Floor(size * 100) / 100);
            }
            else if (fileSize >= 1024)
            {
                var size = fileSize / 1024;
                return string.Format("{0:####0.00} KB",
                    Math.Floor(size * 100) / 100);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }
        }
    }
}