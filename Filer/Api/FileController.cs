using Filer.Api.Shared;
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
        public IActionResult Image([FromRoute] int worknum, [FromRoute] string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
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
        public IActionResult Video([FromRoute] int worknum, [FromRoute] string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
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

        [HttpGet("audio/{worknum}/{*path}")]
        public IActionResult Audio([FromRoute] int worknum, [FromRoute] string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
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
        public IActionResult Download([FromRoute] int worknum, [FromRoute] string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
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

        protected static readonly Dictionary<string, bool> _subtitlesExt = new()
        {
            [".srt"] = true,
            [".ssa"] = true,
            [".ass"] = true,
            [".sub"] = true,
            [".smi"] = true,
            [".txt"] = true,
            [".idx"] = true,
            [".mpl"] = true,
            [".vtt"] = true,
            [".psb"] = true,
            [".sami"] = true,
            [".pjs"] = true,
            [".sup"] = true
        };

        protected static readonly Dictionary<string, bool> _thumbnailExt = new()
        {
            [".jpg"] = true,
            [".png"] = true,
            [".jpeg"] = true,
            [".gif"] = true,
            [".webp"] = true,
            [".bmp"] = true
        };
    }
}