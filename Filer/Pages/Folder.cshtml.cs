using Filer.Extensions;
using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Filer.Pages
{
    public class FolderModel : FolderBasePageModel
    {
        public FolderModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public IActionResult OnGet([FromRoute] int workNum, [FromRoute] string path, [FromQuery] string? search, [FromQuery] string? orderBy)
        {
            var workDir = "";
            var folderPath = "";
            try
            {
                path = path?.Trim('/').Trim('\\') ?? "";
                workDir = _workDirs[workNum - 1].Path;
                folderPath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!Directory.Exists(folderPath))
                    throw new Exception("Path not found.");
                if (!folderPath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
            }
            catch
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = Request.Cookies["orderBy"];

            search = Regex.Replace(search ?? "", @"[<>:""/\\|?*]", "");
            var hasSearch =
                !string.IsNullOrWhiteSpace(search);

            var pathInfo = GetPathInfo(path);
            var dirPath = pathInfo.path;
            var dirName = pathInfo.pathName;
            var parentDirPath = pathInfo.parentPath;
            var parentDirName = pathInfo.parentName;

            var resultLimit = hasSearch ? _searchResultLimit : null as int?;

            var datas = GetFiles(
                workNum, workDir, path, folderPath, search, hasSearch, resultLimit);

            if (!hasSearch)
                datas = UpdateHistoryCount(workNum, path, datas);

            //if (orderBy?.EndsWith("Desc") ?? false)
            //    datas = datas.Reverse();

            var orderDatas = OrderBy(datas, orderBy, hasSearch);

            var isOverResultLimit = false;
            var limitDatas = orderDatas.ToList();
            if (resultLimit != null)
            {
                if (limitDatas.Count > resultLimit)
                {
                    //limitDatas = limitDatas.Take(resultLimit).ToList();
                    limitDatas = limitDatas.GetRange(0, resultLimit.Value);
                    isOverResultLimit = true;
                }
            }

            var data = new
            {
                Host = Request.Host.Value,
                Scheme = Request.Scheme,
                IsAndroid = Request.Headers.UserAgent
                    .ToString().Contains("Android"),
                FileType = string.IsNullOrWhiteSpace(path) ?
                    FileType.WorkDir : FileType.Folder,
                WorkNum = workNum,
                WorkDir = GetWorkDirName(workNum),
                Path = path,
                DirPath = dirPath,
                DirName = dirName,
                ParentDirPath = parentDirPath,
                ParentDirName = parentDirName,
                HasSearch = hasSearch,
                //SearchText = search,
                IsOverResultLimit = isOverResultLimit,
                IsUseVariantSearch = _useVariantSearch,
                Datas = limitDatas,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);

            var encodeData = new
            {
                SearchText = search,
                SearchTextTW = _useVariantSearch ?
                    CachedChineseConverter.ToTraditional(search) : ""
            };
            EncodeData = System.Text.Json.JsonSerializer.Serialize(encodeData, _jsonOptions);
            var title = GetDirNameTitle(dirName, data.WorkDir);
            Title = !hasSearch ? title : $"{search} ({title})";
            return Page();
        }
    }
}