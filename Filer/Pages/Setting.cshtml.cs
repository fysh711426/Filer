using Filer.Pages.Shared;

namespace Filer.Pages
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