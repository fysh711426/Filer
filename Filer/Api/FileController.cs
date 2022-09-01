using Microsoft.AspNetCore.Mvc;
using MimeTypes;

namespace Filer.Api
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

            var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(filePath));
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, mimeType, true);
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
            return File(fs, "application/octet-stream", 
                Path.GetFileName(path), true);
        }
    }
}