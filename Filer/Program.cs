using Filer.Extensions;
using Filer.Pages.Shared;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Filer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddRouting(routeOptions => 
                routeOptions.LowercaseUrls = true);
            builder.Services.AddSingleton(
                HtmlEncoder.Create(allowedRanges: new[] {
                    UnicodeRanges.All }));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<UrlHelperEx>();

            builder.Configuration
                .AddJsonFile("localization.json", optional: true, reloadOnChange: true);

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var configuration = app.Configuration;
            LayoutModel.version = configuration.GetValue<string>("Version") ?? "";
            LayoutModel.baseHref = configuration.GetValue<string>("BaseHref") ?? "";
            
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapRazorPages();
            app.MapControllers();
            app.Run();
        }
    }
}