using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class VideoModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public VideoModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string VideoPath { get; set; } = "";
        public void OnGet(string path)
        {
            VideoPath = path;
        }
    }
}
