using Filer.Pages.Shared;

namespace Filer.Pages
{
    public class LicenseModel : BasePageModel
    {
        public LicenseModel(
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