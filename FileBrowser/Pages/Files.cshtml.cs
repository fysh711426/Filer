using FileBrowser.Models;
using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeTypes;

namespace FileBrowser.Pages
{
    public class FilesModel : BasePageModel
    {
        public FilesModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public List<FileModel> Files { get; set; } = new List<FileModel>();

        public string DirPath { get; set; } = "";
        public string DirName { get; set; } = "";
        public string ParentDirPath { get; set; } = "";
        public string ParentDirName { get; set; } = "";
        public string Host { get; set; } = "";
        public string Scheme { get; set; } = "";
        public bool IsAndroid { get; set; } = false;
        public int WorkNum { get; set; } = 1;

        public IActionResult OnGet([FromRoute] int worknum, [FromRoute] string path = "")
        {
            var workDir = "";
            var folderPath = "";
            try
            {
                WorkNum = worknum;
                workDir = _workDirs[worknum - 1].Path;
                folderPath = Path.Combine(workDir, path);
                if (!Directory.Exists(folderPath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var pathInfo = GetPathInfo(worknum, path);
            DirPath = pathInfo.filePath;
            DirName = pathInfo.fileName;
            ParentDirPath = pathInfo.parentPath;
            ParentDirName = pathInfo.parentName;
            Host = Request.Host.Value;
            Scheme = Request.Scheme;
            IsAndroid = Request.Headers.UserAgent.ToString().Contains("Android");

            var folders = Directory.GetDirectories(folderPath)
                .Select(it => it.Replace(workDir, ""));
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
                    Files.Add(file);
                }
                catch { }
            }

            var files = Directory.GetFiles(folderPath)
                .Select(it => it.Replace(workDir, "")).ToList();
            foreach (var item in files)
            {
                var model = new FileModel();
                model.Path = item;
                model.Name = Path.GetFileName(item);

                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Image;
                    model.MimeType = mimeType;
                    Files.Add(model);
                    continue;
                }
                if (_videoMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Video;
                    model.MimeType = mimeType;
                    var fileSize = new FileInfo(
                        Path.Combine(workDir, item)).Length;
                    model.FileSize = FormatFileSize(fileSize);
                    Files.Add(model);
                    continue;
                }
                if (_audioMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Audio;
                    model.MimeType = mimeType;
                    var fileSize = new FileInfo(
                        Path.Combine(workDir, item)).Length;
                    model.FileSize = FormatFileSize(fileSize);
                    Files.Add(model);
                    continue;
                }
                if (_textMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Text;
                    model.MimeType = mimeType;
                    var fileSize = new FileInfo(
                        Path.Combine(workDir, item)).Length;
                    model.FileSize = FormatFileSize(fileSize);
                    Files.Add(model);
                    continue;
                }
                Files.Add(model);
            }

            Files = Files
                .Select(it => new
                {
                    order = it.FileType == FileType.Other ?
                        int.MaxValue : (int)it.FileType,
                    file = it
                })
                .OrderBy(it => it.order)
                .Select(it => it.file)
                .ToList();

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
