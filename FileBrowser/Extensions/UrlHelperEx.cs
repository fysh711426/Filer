using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FileBrowser.Extensions
{
    public class UrlHelperEx
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UrlHelperEx(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /*
           public virtual string Content(string contentPath)
           {
               if (string.IsNullOrEmpty(contentPath))
               {
                   return null;
               }
               else if (contentPath[0] == '~')
               {
                   var segment = new PathString(contentPath.Substring(1));
                   var applicationPath = HttpContext.Request.PathBase;
           
                   return applicationPath.Add(segment).Value;
               }
           
               return contentPath;
           }
        */
        public string PageRouteLink(string route, params string[] parameters)
        {
            var urlPath = _httpContextAccessor?
                .HttpContext?.Request?.PathBase ?? "";
            urlPath = urlPath.Add($"/{route}");
            var url = new StringBuilder();
            url.Append(urlPath.Value);
            foreach (var param in parameters)
                url.Append($"/{Uri.EscapeDataString(param)}");
            return url.ToString();
        }
    }
}