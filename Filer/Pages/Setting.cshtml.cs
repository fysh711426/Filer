using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public IActionResult OnGet()
        {
            var data = new
            {
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);
            Title = _localization.Settings;
            return Page();
        }
    }
}