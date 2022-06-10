using System.Text;

namespace FileBrowser.Extensions
{
    public static class Helper
    {
        public static string PageRouteLink(string pageName, params string[] parameters)
        {
            var url = new StringBuilder();
            url.Append($"/{pageName}");
            foreach(var param in parameters)
                url.Append($"/{Uri.EscapeDataString(param)}");
            return url.ToString();
        }
    }
}
