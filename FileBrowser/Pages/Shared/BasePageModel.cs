using FileBrowser.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace FileBrowser.Pages.Shared
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
    }
}