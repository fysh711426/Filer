using Filer.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult History(int worknum, string path)
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

            if (_useHistory)
            {
                var historyDir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "History");
                if (!Directory.Exists(historyDir))
                    Directory.CreateDirectory(historyDir);

                var md5 = filePath.ToMD5();
                var historyPath = Path.Combine(historyDir, md5);
                if (!System.IO.File.Exists(historyPath))
                {
                    using (var fs = new FileStream(historyPath, FileMode.Create))
                    {
                        //空白檔案
                    }
                }
            }
            return Ok();
        }
    }
}