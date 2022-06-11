using FileBrowser.Models;
using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class WorkDirModel : BasePageModel
    {
        public WorkDirModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public List<WorkDir> WorkDirs { get; set; } = new List<WorkDir>();
        public void OnGet()
        {
            WorkDirs = _workDirs;
        }
    }
}
