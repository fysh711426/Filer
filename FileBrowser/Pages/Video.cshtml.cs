using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class VideoModel : BasePageModel
    {
        public VideoModel(IConfiguration configuration)
            : base(configuration)
        {
        }

        public string Title { get; set; } = "";
        public string VideoPath { get; set; } = "";
        public int WorkNum { get; set; } = 1;
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

            Title = Path.GetFileName(path);
            VideoPath = path;

            return Page();
        }
    }
}
