using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;

namespace FileBrowser.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseController
    {
        public FileController(IConfiguration configuration)
            : base(configuration)
        {
        }

        [HttpGet("image/{worknum}/{*path}")]
        public IActionResult Image(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet-stream", Path.GetFileName(path));
        }

        [HttpGet("video/{worknum}/{*path}")]
        public IActionResult Video(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "video/mp4", true);
        }

        [HttpGet("audio/{worknum}/{*path}")]
        public IActionResult Audio(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "audio/mpeg", true);
        }

        [HttpGet("thumbnail/{worknum}/{*path}")]
        public IActionResult Thumbnail(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var arguments = $@"-ss 00:00:01.00 -i ""{filePath}"" -vf ""scale=320:240:force_original_aspect_ratio=decrease"" -vframes 1 -f image2 pipe: -loglevel error";
            var info = new ProcessStartInfo("ffmpeg.exe", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            var process = Process.Start(info) ?? 
                throw new Exception("Process is null.");
            return File(process.StandardOutput.BaseStream,
                "image/jpeg", $"{Path.GetFileNameWithoutExtension(path)}.thumbnail.jpg",
                new DateTimeOffset(lastModified), entityTag);
        }

        [HttpGet("download/{worknum}/{*path}")]
        public IActionResult Download(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet-stream", true);
        }
    }
}