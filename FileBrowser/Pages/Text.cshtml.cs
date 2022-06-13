using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class TextModel : BasePageModel
    {
        public TextModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public string ParentDirPath { get; set; } = "";
        public string ParentDirName { get; set; } = "";
        public int WorkNum { get; set; } = 1;
        public string Context { get; set; } = "";

        public IActionResult OnGet([FromRoute] int worknum, [FromRoute] string path)
        {
            var workDir = "";
            var filePath = "";
            try
            {
                WorkNum = worknum;
                workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var pathInfo = GetPathInfo(worknum, path);
            FilePath = pathInfo.filePath;
            FileName = pathInfo.fileName;
            ParentDirPath = pathInfo.parentPath;
            ParentDirName = pathInfo.parentName;

            Context = System.IO.File.ReadAllText(filePath);

            return Page();
        }
    }
}
