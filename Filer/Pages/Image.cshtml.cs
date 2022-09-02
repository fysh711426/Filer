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

        public IActionResult OnGet([FromRoute] int workNum, [FromRoute] string path)
        {
            var workDir = "";
            var filePath = "";
            try
            {
                workDir = _workDirs[workNum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var pathInfo = GetPathInfo(workNum, path);

            var folderPath = Path.Combine(workDir, pathInfo.parentPath);

            var datas = new List<FileModel>();

            var files = Directory.GetFiles(folderPath)
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));
            foreach (var item in files)
            {
                var model = new FileModel();
                model.Path = item;
                model.Name = Path.GetFileName(item);
                model.FileSize = FormatFileSize(new FileInfo(
                    Path.Combine(workDir, item)).Length);

                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Image;
                    model.MimeType = mimeType;
                    datas.Add(model);
                }
            }

            var data = new
            {
                WorkNum = workNum,
                FilePath = pathInfo.path,
                FileName = pathInfo.pathName,
                ParentDirPath = pathInfo.parentPath,
                ParentDirName = pathInfo.parentName,
                Datas = datas
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}