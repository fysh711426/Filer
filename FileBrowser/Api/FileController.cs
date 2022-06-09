using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;

namespace FileBrowser.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IConfiguration _configuration;
        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("image/{path}")]
        public IActionResult Image(string path)
        {
            var baseDir = _configuration["BaseDir"].TrimEnd('\\');
            var filePath = Path.Combine(baseDir, path);

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet-stream", Path.GetFileName(path));
        }

        [HttpGet("video/{path}")]
        public IActionResult Video(string path)
        {
            var baseDir = _configuration["BaseDir"].TrimEnd('\\');
            var filePath = Path.Combine(baseDir, path);

            try
            {
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fs, "video/mp4", true);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("thumbnail/{path}")]
        public IActionResult Thumbnail(string path)
        {
            var baseDir = _configuration["BaseDir"].TrimEnd('\\');
            var filePath = Path.Combine(baseDir, path);
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
    }
}
