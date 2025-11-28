using Filer.Extensions;
using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using Newtonsoft.Json;
using System.Data;

namespace Filer.Pages
{
    public class ImageModel : BasePageModel
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

            var datas = new List<FileModel>();

            var files = Directory.GetFiles(folderPath)
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));

            if (!_useWindowsNaturalSort)
                if (orderBy == "nameDesc")
                    files = files.Reverse();

            foreach (var item in files)
            {
                var model = new FileModel();
                model.Path = item;
                model.Name = Path.GetFileName(item);
                model.FileSize = FormatFileSize(new FileInfo(
                    Path.Combine(workDir, item)).Length);

                var fullPath = Path.GetFullPath(Path.Combine(workDir, item));
                if (_useHistory)
                {
                    var historyDir = GetAppDirectory("History");
                    var parentDir = Path.Combine($"{workNum}", Path.GetDirectoryName(item) ?? "");
                    if (!string.IsNullOrWhiteSpace(parentDir))
                    {
                        var historySubDir = Path.GetFullPath(Path.Combine(historyDir, parentDir));
                        if (historySubDir.StartsWith(historyDir))
                        {
                            var historyPath = Path.Combine(historySubDir, fullPath.ToMD5());
                            if (System.IO.File.Exists(historyPath))
                                model.HasHistory = true;
                        }
                    }
                }

                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Image;
                    model.MimeType = mimeType;
                    datas.Add(model);
                }
            }

            var orderDatas = datas
                .OrderBy(it => it.FileType);

            if (orderBy == "date")
                orderDatas = orderDatas.ThenBy(it => it.LastWriteTimeUtc);
            else if (orderBy == "dateDesc")
                orderDatas = orderDatas.ThenByDescending(it => it.LastWriteTimeUtc);
            else if (orderBy == "size")
                orderDatas = orderDatas.ThenBy(it => it.FileLength);
            else if (orderBy == "sizeDesc")
                orderDatas = orderDatas.ThenByDescending(it => it.FileLength);
            else
            {
                if (_useWindowsNaturalSort)
                {
                    if (orderBy == "nameDesc")
                        orderDatas = orderDatas.ThenByDescending(it => it.Name,
                            new WindowsNaturalSort());
                    else
                        orderDatas = orderDatas.ThenBy(it => it.Name,
                            new WindowsNaturalSort());
                }
            }

            datas = orderDatas.ToList();

            var data = new
            {
                WorkNum = workNum,
                FilePath = pathInfo.path,
                FileName = pathInfo.pathName,
                ParentDirPath = pathInfo.parentPath,
                ParentDirName = pathInfo.parentName,
                Datas = datas,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}