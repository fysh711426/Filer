using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using static Filer.Extensions.PathHelper;

namespace Filer.Pages
{
    public class ImageModel : FolderBasePageModel
    {
        public ImageModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public IActionResult OnGet([FromRoute] int workNum, [FromRoute] string path, [FromQuery] string? orderBy)
        {
            var workDir = "";
            var filePath = "";
            try
            {
                path = path?.Trim('/').Trim('\\') ?? "";
                workDir = _workDirs[workNum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = Request.Cookies["orderBy"];

            var pathInfo = GetPathInfo(path);
            var folderPath = Path.GetFullPath(
                Path.Combine(workDir, pathInfo.parentPath));

            var datas = _folderService.EnumerateFiles(workNum, workDir, folderPath)
                .Where(it => it.FileType == FileType.Image);

            //var datas = EnumerateEntries(workNum, workDir, folderPath)
            //    .Where(it => it.FileType == FileType.Image);

            //if (orderBy?.EndsWith("Desc") ?? false)
            //    datas = datas.Reverse();

            var orderDatas = _folderService.OrderBy(datas, orderBy);

            var data = new
            {
                FileType = FileType.Image,
                WorkNum = workNum,
                WorkDir = GetWorkDirName(workNum),
                Path = path,
                FilePath = pathInfo.path,
                FileName = pathInfo.pathName,
                ParentDirPath = pathInfo.parentPath,
                ParentDirName = pathInfo.parentName,
                Datas = orderDatas.ToList(),
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            Title = pathInfo.pathName;
            return Page();
        }
    }
}