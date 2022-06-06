using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace FileBrowser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddSingleton(
                HtmlEncoder.Create(allowedRanges: new[] {
                    UnicodeRanges.All }));

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapRazorPages();
            app.MapControllers();
            app.Run();
        }
    }
}