using Filer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace Filer.Api.Shared
{
    public class BaseController : Controller
    {
        protected static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        protected static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected static readonly EnumerationOptions _enumerationOptions = new()
        {
            MatchType = MatchType.Simple,
            AttributesToSkip = FileAttributes.None,
            IgnoreInaccessible = true
        };

        protected static readonly EnumerationOptions _enumerationRecursiveOptions = new()
        {
            MatchType = MatchType.Simple,
            AttributesToSkip = FileAttributes.None,
            IgnoreInaccessible = true,
            RecurseSubdirectories = true
        };

        protected readonly IConfiguration _configuration;
        protected readonly bool _usePreviewCache;
        protected readonly bool _useThumbnailCache;
        protected readonly bool _useHistory;
        protected readonly bool _useSearchAsync;
        protected readonly bool _useVariantSearch;
        protected readonly int _searchResultLimit;
        protected readonly List<WorkDir> _workDirs;
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            _usePreviewCache = _configuration.GetValue<bool>("UsePreviewCache");
            _useThumbnailCache = _configuration.GetValue<bool>("UseThumbnailCache");
            _useHistory = _configuration.GetValue<bool>("UseHistory");
            _useSearchAsync = _configuration.GetValue<bool>("UseSearchAsync");
            _useVariantSearch = _configuration.GetValue<bool>("UseVariantSearch");
            _searchResultLimit = _configuration.GetValue<int?>("SearchResultLimit") ?? 1000;
            var workDirs = _configuration
                .GetSection("WorkDirs").Get<WorkDir[]>() ?? Array.Empty<WorkDir>();
            var index = 0;
            foreach (var item in workDirs)
            {
                item.Path = item.Path.TrimEnd(Path.DirectorySeparatorChar);
                item.Path = $@"{item.Path}{Path.DirectorySeparatorChar}";
                item.IsPathError = !Directory.Exists(item.Path);
                item.Index = index++;
            }
            _workDirs = workDirs.ToList();
        }

        protected string GetWorkDirName(int workNum)
        {
            return _workDirs[workNum - 1].Name;
        }

        protected async Task WriteToBody(Stream stream)
        {
            var read = 0;
            var buffer = new byte[1024 * 1024];
            while (true)
            {
                read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read <= 0)
                    break;
                await Response.Body.WriteAsync(buffer, 0, read);
            }
        }
    }
}