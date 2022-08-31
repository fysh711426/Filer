using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public IActionResult OnGet()
        {
            var data = new
            {
                Datas = _workDirs
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}