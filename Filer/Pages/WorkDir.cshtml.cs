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
                            it.ItemCount = fileCount + folderCount;

                            if (_useHistory)
                            {
                                var historyDir = GetAppDirectory("History");
                                var folderDir = $"{it.Index}";
                                var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                                if (historySubDir.StartsWith(historyDir))
                                {
                                    var historyPath = Path.Combine(historySubDir, "HistoryCount");
                                    if (System.IO.File.Exists(historyPath))
                                    {
                                        var countText = System.IO.File.ReadAllText(historyPath);
                                        if (int.TryParse(countText, out var count))
                                            it.HistoryCount = count;
                                    }
                                }
                                if (it.HistoryCount == fileCount + folderCount)
                                    it.HasHistory = true;
                            }
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
                    it.ItemCount,
                    it.HistoryCount,
                    it.HasHistory
                })
                .ToList();

            var data = new
            {
                Datas = workDirs,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}