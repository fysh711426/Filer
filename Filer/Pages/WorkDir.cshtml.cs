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
                .Select(it =>
                {
                    try
                    {
                        if (!it.IsPathError)
                        {
                            var itemPath = Path.GetFullPath(it.Path);
                            var fileCount = Directory.GetFiles(itemPath).Length;
                            var folderCount = Directory.GetDirectories(itemPath).Length;
                            it.ItemCount = $"{(fileCount + folderCount)} ¶µ";
                        }
                    }
                    catch { }
                    return it;
                })
                .Select(it => new
                {
                    it.Name,
                    it.Index,
                    it.IsPathError,
                    Path = $"{it.Index + 1}",
                    it.ItemCount
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