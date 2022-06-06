using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string>? folders { get; set; }
        public List<string>? files { get; set; }

        public void OnGet(string path = "")
        {
            var baseDir = _configuration["BaseDir"].TrimEnd('\\');
            var folderPath = Path.Combine(baseDir, path);

            folders = Directory.GetDirectories(folderPath)
                .Select(it => it.Replace($@"{baseDir}\", "")).ToList();
            files = Directory.GetFiles(folderPath)
                .Select(it => it.Replace($@"{baseDir}\", "")).ToList();
        }
    }
}
