using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Filer.Pages
{
    public class TextModel : BasePageModel
    {
        public TextModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public string Context { get; set; } = "";
        public IActionResult OnGet([FromRoute] int workNum, [FromRoute] string path)
        {
            var workDir = "";
            var filePath = "";
            try
            {
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

            var pathInfo = GetPathInfo(workNum, path);

            var data = new
            {
                WorkNum = workNum,
                FilePath = pathInfo.path,
                FileName = pathInfo.pathName,
                ParentDirPath = pathInfo.parentPath,
                ParentDirName = pathInfo.parentName,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            Context = System.IO.File.ReadAllText(filePath);
            return Page();
        }
    }
}