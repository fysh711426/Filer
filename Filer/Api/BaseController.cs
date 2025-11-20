using Filer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filer.Api
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected readonly bool _usePreviewCache;
        protected readonly bool _useHistory;
        protected readonly List<WorkDir> _workDirs;
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            _usePreviewCache = _configuration.GetValue<bool>("UsePreviewCache");
            _useHistory = _configuration.GetValue<bool>("UseHistory");
            var workDirs = _configuration
                .GetSection("WorkDirs").Get<WorkDir[]>();
            var index = 0;
            foreach (var item in workDirs)
            {
                item.Path = item.Path.TrimEnd(Path.DirectorySeparatorChar);
                item.Path = $@"{item.Path}{Path.DirectorySeparatorChar}";
                item.IsPathError = !Directory.Exists(item.Path);
                item.Index = index++;
            }
            _workDirs = workDirs.ToList();
        }

        protected async Task WriteToBody(Stream stream)
        {
            var read = 0;
            var buffer = new byte[1024 * 1024];
            while (true)
            {
                read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read <= 0)
                    break;
                await Response.Body.WriteAsync(buffer, 0, read);
            }
        }
    }
}