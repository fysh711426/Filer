using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Filer.Extensions.PathHelper;

namespace Filer.Pages
{
    public class AudioModel : BasePageModel
    {
        public AudioModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public IActionResult OnGet([FromRoute] int workNum, [FromRoute] string path)
        {
            var workDir = "";
            var filePath = "";
            try
            {
                path = path?.Trim('/').Trim('\\') ?? "";
                workDir = _workDirs[workNum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
            }
            catch
            {
                return NotFound();
            }

            var pathInfo = GetPathInfo(path);

            var data = new
            {
                FileType = FileType.Audio,
                WorkNum = workNum,
                WorkDir = GetWorkDirName(workNum),
                Path = path,
                FilePath = pathInfo.path,
                FileName = pathInfo.pathName,
                ParentDirPath = pathInfo.parentPath,
                ParentDirName = pathInfo.parentName,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            Title = pathInfo.pathName;
            return Page();
        }
    }
}