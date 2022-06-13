using FileBrowser.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace FileBrowser.Pages.Shared
{
    public class BasePageModel : PageModel
    {
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

        protected (
            string parentPath,
            string parentName,
            string filePath,
            string fileName) GetPathInfo(int worknum, string path)
        {
            var filePath = path.Trim(Path.DirectorySeparatorChar);
            var parentPath = Path.GetDirectoryName(filePath) ?? "";
            return (
                parentPath, GetFileName(worknum, parentPath),
                filePath, GetFileName(worknum, filePath)
            );
        }

        private string GetFileName(int worknum, string path)
        {
            if (path == "")
                return _workDirs[worknum - 1].Name;
            return Path.GetFileName(path);
        }
    }
}
