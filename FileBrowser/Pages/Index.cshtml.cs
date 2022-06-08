using FileBrowser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeTypes;

namespace FileBrowser.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<FileModel> Folders { get; set; } = new List<FileModel>();
        public List<FileModel> Images { get; set; } = new List<FileModel>();
        public List<FileModel> Videos { get; set; } = new List<FileModel>();
        public List<FileModel> Texts { get; set; } = new List<FileModel>();
        public List<FileModel> Others { get; set; } = new List<FileModel>();

        public void OnGet(string path = "")
        {
            var baseDir = _configuration["BaseDir"].TrimEnd('\\');
            var folderPath = Path.Combine(baseDir, path);

            Folders = Directory.GetDirectories(folderPath)
                .Select(it => it.Replace($@"{baseDir}\", ""))
                .Select(it => new FileModel
                {
                    Path = it,
                    Name = Path.GetFileName(it),
                    ItemCount = 
                        (Directory.GetFiles(Path.Combine(baseDir, it)).Length +
                         Directory.GetDirectories(Path.Combine(baseDir, it)).Length)
                            .ToString() + " ¶µ"
                }).ToList();

            var files = Directory.GetFiles(folderPath)
                .Select(it => it.Replace($@"{baseDir}\", "")).ToList();
            foreach(var item in files)
            {
                var model = new FileModel();
                model.Path = item;
                model.Name = Path.GetFileName(item);
                
                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.MimeType = mimeType;
                    Images.Add(model);
                    continue;
                }
                if (_videoMimeType.ContainsKey(mimeType))
                {
                    model.MimeType = mimeType;
                    var fileSize = new FileInfo(
                        Path.Combine(baseDir, item)).Length;
                    model.FileSize = FormatFileSize(fileSize);
                    Videos.Add(model);
                    continue;
                }
                if (_textMimeType.ContainsKey(mimeType))
                {
                    model.MimeType = mimeType;
                    Texts.Add(model);
                    continue;
                }
                Others.Add(model);
            }
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
