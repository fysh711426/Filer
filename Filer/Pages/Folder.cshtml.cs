using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using Newtonsoft.Json;

namespace Filer.Pages
{
    public class FolderModel : BasePageModel
    {
        public FolderModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public IActionResult OnGet([FromRoute] int workNum, [FromRoute] string path)
        {
            var workDir = "";
            var folderPath = "";
            try
            {
                path = path?.Trim('/') ?? "";
                workDir = _workDirs[workNum - 1].Path;
                folderPath = Path.Combine(workDir, path);
                if (!Directory.Exists(folderPath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var pathInfo = GetPathInfo(workNum, path);
            var dirPath = pathInfo.path;
            var dirName = pathInfo.pathName;
            var parentDirPath = pathInfo.parentPath;
            var parentDirName = pathInfo.parentName;

            var datas = new List<FileModel>();

            var folders = Directory.GetDirectories(folderPath)
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));
            foreach(var item in folders)
            {
                try
                {
                    var file = new FileModel();
                    file.FileType = FileType.Folder;
                    file.Path = item;
                    file.Name = Path.GetFileName(item);
                    var itemPath = Path.Combine(workDir, item);
                    var fileCount = Directory.GetFiles(itemPath).Length;
                    var folderCount = Directory.GetDirectories(itemPath).Length;
                    file.ItemCount = $"{(fileCount + folderCount)} ¶µ";
                    datas.Add(file);
                }
                catch { }
            }

            var files = Directory.GetFiles(folderPath)
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));
            foreach (var item in files)
            {
                var model = new FileModel();
                model.Path = item;
                model.Name = Path.GetFileName(item);
                var filePath = Path.Combine(workDir, item);
                model.FileSize = FormatFileSize(new FileInfo(filePath).Length);
                //var lastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(filePath);
                //var lastWriteTime = lastWriteTimeUtc.ToString("yyyy/MM/dd HH:mm:ss");
                //model.LastWriteTime = lastWriteTime;

                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Image;
                    model.MimeType = mimeType;
                    //var img = Image.Identify(filePath);
                    //model.Width = img.Width;
                    //model.Height = img.Height;
                    datas.Add(model);
                    continue;
                }
                if (_videoMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Video;
                    model.MimeType = mimeType;
                    datas.Add(model);
                    continue;
                }
                if (_audioMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Audio;
                    model.MimeType = mimeType;
                    datas.Add(model);
                    continue;
                }
                if (_textMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Text;
                    model.MimeType = mimeType;
                    datas.Add(model);
                    continue;
                }
                datas.Add(model);
            }

            datas = datas
                .OrderBy(it => it.FileType)
                .ToList();

            var data = new
            {
                Host = Request.Host.Value,
                Scheme = Request.Scheme,
                IsAndroid = Request.Headers.UserAgent
                    .ToString().Contains("Android"),
                WorkNum = workNum,
                DirPath = dirPath,
                DirName = dirName,
                ParentDirPath = parentDirPath,
                ParentDirName = parentDirName,
                Datas = datas
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            return Page();
        }
    }
}