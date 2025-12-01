using Filer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace Filer.Pages.Shared
{
    public class BasePageModel : PageModel
    {
        public string? Data { get; set; } = null;
        public string? EncodeData { get; set; } = null;

        protected static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        protected static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected readonly int _countLimit = 1000;

        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IConfiguration _configuration;
        protected readonly bool _useHistory;
        protected readonly bool _useWindowsNaturalSort;
        protected readonly string _language;
        protected readonly List<WorkDir> _workDirs;
        protected readonly Localization _localization;
        public BasePageModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _useHistory = _configuration.GetValue<bool>("UseHistory");
            _useWindowsNaturalSort = _configuration.GetValue<bool>("UseWindowsNaturalSort");
            _language = _configuration.GetValue<string>("Language") ?? "";
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
            _localization = GetLocalization(_configuration, _language);
        }

        protected string GetWorkDirName(int workNum)
        {
            return _workDirs[workNum - 1].Name;
        }

        protected (string path, string pathName, string parentPath, string parentName, string grandParentPath, string grandParentName)
            GetPathInfo(string path)
        {
            path = path.Trim('/').Trim('\\').Replace(@"\", "/") ?? "";
            var parentPath = string.IsNullOrWhiteSpace(path) ? "" :
                Path.GetDirectoryName(path)?.Replace(@"\", "/") ?? "";
            var grandParentPath = string.IsNullOrWhiteSpace(parentPath) ? "" :
                Path.GetDirectoryName(parentPath)?.Replace(@"\", "/") ?? "";
            return (
                path,
                Path.GetFileName(path),
                parentPath,
                Path.GetFileName(parentPath),
                grandParentPath,
                Path.GetFileName(grandParentPath)
            );
        }

        //----- 舊寫法 -----
        [Obsolete]
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

        [Obsolete]
        private string GetPathName(int workNum, string path)
        {
            if (path == "")
                return _workDirs[workNum - 1].Name;
            return Path.GetFileName(path);
        }
        //----- 舊寫法 -----

        protected string GetAppDirectory(string? combineDir = null)
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(combineDir))
                appDir = Path.Combine(appDir, combineDir);
            appDir = appDir.TrimEnd(Path.DirectorySeparatorChar);
            appDir = $@"{appDir}{Path.DirectorySeparatorChar}";
            return appDir;
        }

        private Localization GetLocalization(IConfiguration configuration, string language)
        {
            return new Localization
            {
                WorkDir = configuration.GetValue<string>($"Localization:{language}:WorkDir") ?? "",
                WorkDirNotSet = configuration.GetValue<string>($"Localization:{language}:WorkDirNotSet") ?? "",
                PathNotFound = configuration.GetValue<string>($"Localization:{language}:PathNotFound") ?? "",
                Items = configuration.GetValue<string>($"Localization:{language}:Items") ?? "",
                Search = configuration.GetValue<string>($"Localization:{language}:Search") ?? "",
                SearchResult = configuration.GetValue<string>($"Localization:{language}:SearchResult") ?? "",
                Browse = configuration.GetValue<string>($"Localization:{language}:Browse") ?? "",
                Thumbnail = configuration.GetValue<string>($"Localization:{language}:Thumbnail") ?? "",
                Download = configuration.GetValue<string>($"Localization:{language}:Download") ?? "",
                Auto = configuration.GetValue<string>($"Localization:{language}:Auto") ?? "",
                Name = configuration.GetValue<string>($"Localization:{language}:Name") ?? "",
                Date = configuration.GetValue<string>($"Localization:{language}:Date") ?? "",
                Size = configuration.GetValue<string>($"Localization:{language}:Size") ?? "",
                Settings = configuration.GetValue<string>($"Localization:{language}:Settings") ?? "",
                Version = configuration.GetValue<string>($"Localization:{language}:Version") ?? "",
                Enable = configuration.GetValue<string>($"Localization:{language}:Enable") ?? "",
                SaveSuccess = configuration.GetValue<string>($"Localization:{language}:SaveSuccess") ?? "",
                UseDeepLinkDescription = configuration.GetValue<string>($"Localization:{language}:UseDeepLinkDescription") ?? "",
                DeepLinkPackageDescriptionFirst = configuration.GetValue<string>($"Localization:{language}:DeepLinkPackageDescriptionFirst") ?? "",
                DeepLinkPackageDescriptionSecond = configuration.GetValue<string>($"Localization:{language}:DeepLinkPackageDescriptionSecond") ?? "",
                SearchInputErrorMessage = configuration.GetValue<string>($"Localization:{language}:SearchInputErrorMessage") ?? "",
                OverCountLimitErrorMessage = configuration.GetValue<string>($"Localization:{language}:OverCountLimitErrorMessage") ?? ""
            };
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
            ["video/mp4"] = true,
            ["video/mov"] = true,
            ["video/avi"] = true,
            ["video/wmv"] = true,
            ["video/webm"] = true,
            ["video/vnd.dlna.mpeg-tts"] = true
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