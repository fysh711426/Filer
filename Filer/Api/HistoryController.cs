using Filer.Api.Shared;
using Filer.Extensions;
using Microsoft.AspNetCore.Mvc;
using static Filer.Extensions.PathHelper;

namespace Filer.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : BaseController
    {
        public HistoryController(IConfiguration configuration)
            : base(configuration)
        {
        }

        [HttpGet("{worknum}/{*path}")]
        public IActionResult History([FromRoute] int worknum, [FromRoute] string path)
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

            if (_useHistory)
            {
                var historyDir = GetAppDirectory("History");
                var parentDir = Path.Combine($"{worknum}", Path.GetDirectoryName(path) ?? "");
                if (string.IsNullOrWhiteSpace(parentDir))
                    throw new Exception("HistoryPath is not found parentDir.");

                var historySubDir = Path.GetFullPath(Path.Combine(historyDir, parentDir));
                if (!historySubDir.StartsWith(historyDir))
                    throw new Exception("HistoryPath is outside of the historyDir.");

                if (!Directory.Exists(historySubDir))
                    Directory.CreateDirectory(historySubDir);

                var md5 = filePath.ToMD5();
                var historyPath = Path.Combine(historySubDir, md5);
                if (!System.IO.File.Exists(historyPath))
                {
                    using (var fs = new FileStream(historyPath, FileMode.Create))
                    {
                        // empty file
                    }
                }
            }
            return Ok();
        }

        [HttpPost("clear/{worknum}/{*path}")]
        public IActionResult Clear([FromRoute] int worknum, [FromRoute] string? path, [FromBody] bool isClearSubdirectory)
        {
            var workDir = "";
            var folderPath = "";
            try
            {
                path = path?.Trim('/').Trim('\\') ?? "";
                workDir = _workDirs[worknum - 1].Path;
                folderPath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!folderPath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
                if (!Directory.Exists(folderPath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            if (_useHistory)
            {
                var historyDir = GetAppDirectory("History");
                var folderDir = Path.Combine($"{worknum}", path);
                var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                if (!historySubDir.StartsWith(historyDir))
                    throw new Exception("HistoryPath is outside of the historyDir.");
                historySubDir = $"{historySubDir}{Path.DirectorySeparatorChar}";

                var files = Directory.EnumerateFiles(historySubDir, "*",
                    isClearSubdirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                foreach(var item in files)
                {
                    if (item.StartsWith(historySubDir))
                    {
                        System.IO.File.Delete(item);
                    }
                }
            }
            return Ok();
        }
    }
}