using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Filer.Pages
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
            var workDirs = _workDirs
                .Select(it => new
                {
                    it.Name,
                    it.Index,
                    it.IsPathError,
                    Path = $"{it.Index + 1}"
                })
                .ToList();

            var data = new
            {
                Datas = workDirs
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}