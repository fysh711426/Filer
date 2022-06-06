using Microsoft.AspNetCore.Mvc;

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
            var folderPath = Path.Combine(baseDir, path);

            var fs = new FileStream(folderPath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet-stream", Path.GetFileName(path));
        }
    }
}
