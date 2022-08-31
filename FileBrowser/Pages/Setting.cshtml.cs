using FileBrowser.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBrowser.Pages
{
    public class SettingModel : BasePageModel
    {
        public SettingModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public void OnGet()
        {
        }
    }
}