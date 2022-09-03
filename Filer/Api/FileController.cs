using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
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
            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(filePath));
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, mimeType, 
                new DateTimeOffset(lastModified), entityTag, true);
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

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "video/mp4", 
                new DateTimeOffset(lastModified), entityTag, true);
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

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "audio/mpeg", 
                new DateTimeOffset(lastModified), entityTag, true);
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

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet-stream", Path.GetFileName(path), 
                new DateTimeOffset(lastModified), entityTag, true);
        }
    }
}