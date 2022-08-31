using FileBrowser.Models;
using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeTypes;
using Newtonsoft.Json;

namespace FileBrowser.Pages
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
                model.FileSize = FormatFileSize(new FileInfo(
                    Path.Combine(workDir, item)).Length);

                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Image;
                    model.MimeType = mimeType;
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

        private readonly Dictionary<string, bool> _imageMimeType = new()
        {
            ["image/jpeg"] = true,
            ["image/png"] = true,
            ["image/gif"] = true
        };

        private readonly Dictionary<string, bool> _videoMimeType = new()
        {
            ["video/mp4"] = true
        };

        private readonly Dictionary<string, bool> _audioMimeType = new()
        {
            ["audio/mpeg"] = true
        };

        private readonly Dictionary<string, bool> _textMimeType = new()
        {
            ["text/plain"] = true
        };

        private static string FormatFileSize(double fileSize)
        {
            if (fileSize < 0)
            {
                return "Error";
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                return string.Format("{0:########0.00} GB", ((double)fileSize) / (1024 * 1024 * 1024));
            }
            else if (fileSize >= 1024 * 1024)
            {
                return string.Format("{0:####0.00} MB", ((double)fileSize) / (1024 * 1024));
            }
            else if (fileSize >= 1024)
            {
                return string.Format("{0:####0.00} KB", ((double)fileSize) / 1024);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }
        }
    }
}