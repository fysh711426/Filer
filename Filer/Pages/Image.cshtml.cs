using Filer.Extensions;
using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using Newtonsoft.Json;
using System.Data;

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
                workDir = _workDirs[workNum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
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

            var datas = EnumerateFiles(workNum, workDir, folderPath)
                .Where(it => it.FileType == FileType.Image);

            //if (orderBy?.EndsWith("Desc") ?? false)
            //    datas = datas.Reverse();

            var orderDatas = OrderBy(datas, orderBy);

            var data = new
            {
                WorkNum = workNum,
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