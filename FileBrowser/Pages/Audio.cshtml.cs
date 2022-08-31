using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FileBrowser.Pages
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
                workDir = _workDirs[workNum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
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
                ParentDirName = pathInfo.parentName
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}