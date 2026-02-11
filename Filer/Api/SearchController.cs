using Filer.Extensions;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Filer.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : FolderBaseController
    {
        protected static readonly JsonSerializerOptions _ndjsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public SearchController(IConfiguration configuration)
            : base(configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> StreamWorkDirResults([FromQuery] string? search)
        {
            search = Regex.Replace(search ?? "", @"[<>:""/\\|?*]", "");
            var hasSearch =
                !string.IsNullOrWhiteSpace(search);

            var resultLimit = hasSearch ? _searchResultLimit : null as int?;

            var workDirs = _folderService.GetWorkDirs().ToList();

            var datas = workDirs.AsEnumerable();
            if (hasSearch)
                datas = datas.Where(it =>
                    it.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (hasSearch)
                datas = datas.Concat(
                    _folderService.GetWorkDirsFiles(search, hasSearch, resultLimit));

            Response.ContentType = "application/x-ndjson";

            await using (var writer = new StreamWriter(HttpContext.Response.Body, Encoding.UTF8, leaveOpen: true))
            {
                foreach (var item in datas)
                {
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                        break;
                    await writer.WriteLineAsync(JsonSerializer.Serialize(item, _ndjsonOptions));
                    await writer.FlushAsync();
                }
            }
            if (hasSearch && _useVariantSearch)
                CachedChineseConverter.SaveCache();
            return new EmptyResult();
        }

        [HttpGet("{worknum}/{*path}")]
        public async Task<IActionResult> StreamFolderResults([FromRoute] int workNum, [FromRoute] string? path, [FromQuery] string? search)
        {
            var workDir = "";
            var folderPath = "";
            try
            {
                path = path?.Trim('/').Trim('\\') ?? "";
                workDir = _workDirs[workNum - 1].Path;
                folderPath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!folderPath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
                if (!Directory.Exists(folderPath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            search = Regex.Replace(search ?? "", @"[<>:""/\\|?*]", "");
            var hasSearch =
                !string.IsNullOrWhiteSpace(search);

            var resultLimit = hasSearch ? _searchResultLimit : null as int?;

            var datas = _folderService.GetFiles(
                workNum, workDir, path, folderPath, search, hasSearch, resultLimit);

            if (!hasSearch)
                datas = _folderService.UpdateHistoryCount(workNum, path, datas);

            Response.ContentType = "application/x-ndjson";

            await using(var writer = new StreamWriter(HttpContext.Response.Body, Encoding.UTF8, leaveOpen: true))
            {
                foreach (var item in datas)
                {
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                        break;
                    await writer.WriteLineAsync(JsonSerializer.Serialize(item, _ndjsonOptions));
                    await writer.FlushAsync();
                }
            }
            if (hasSearch && _useVariantSearch)
                CachedChineseConverter.SaveCache();
            return new EmptyResult();
        }
    }
}