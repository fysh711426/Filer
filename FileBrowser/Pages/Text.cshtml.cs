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

        public string Title { get; set; } = "";
        public string Context { get; set; } = "";

        public IActionResult OnGet([FromRoute] int worknum, [FromRoute] string path)
        {
            var workDir = "";
            var filePath = "";
            try
            {
                workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            Title = Path.GetFileName(filePath);
            Context = System.IO.File.ReadAllText(filePath);

            return Page();
        }
    }
}
