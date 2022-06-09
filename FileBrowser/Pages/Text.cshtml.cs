using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class TextModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public TextModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Title { get; set; } = "";
        public string Context { get; set; } = "";

        public void OnGet(string path)
        {
            var baseDir = _configuration["BaseDir"].TrimEnd('\\');
            var filePath = Path.Combine(baseDir, path);

            Title = Path.GetFileName(filePath);
            Context = System.IO.File.ReadAllText(filePath);
        }
    }
}
