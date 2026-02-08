using System.Text;

namespace Filer.Extensions
{
    public static class RouteHelper
    {
        public static string RouteLink(params string[] args)
        {
            var url = new StringBuilder();
            foreach (var param in args)
            {
                var splits = param.Split('/');
                foreach (var s in splits)
                {
                    url.Append("/" + Uri.EscapeDataString(s));
                }
            }
            return url.ToString().Trim('/');
        }
    }
}