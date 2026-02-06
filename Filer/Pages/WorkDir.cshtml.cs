using Filer.Extensions;
using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Filer.Pages
{
    public class WorkDirModel : FolderBasePageModel
    {
        public WorkDirModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        public IActionResult OnGet([FromQuery] string? search, [FromQuery] string? orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = Request.Cookies["orderBy"];

            search = Regex.Replace(search ?? "", @"[<>:""/\\|?*]", "");
            var hasSearch =
                !string.IsNullOrWhiteSpace(search);

            var isOverResultLimit = false;
            var limitDatas = new List<FileModel>();

            if (!hasSearch || !_useSearchAsync)
            {
                var resultLimit = hasSearch ? _searchResultLimit : null as int?;

                var workDirs = _folderService.GetWorkDirs().ToList();

                var datas = workDirs.AsEnumerable();
                if (hasSearch)
                    datas = datas.Where(it =>
                        it.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

                if (hasSearch)
                    datas = datas.Concat(
                        _folderService.GetWorkDirsFiles(search, hasSearch, resultLimit));

                //if (orderBy?.EndsWith("Desc") ?? false)
                //    datas = datas.Reverse();

                var orderDatas = _folderService.OrderBy(datas, orderBy, hasSearch);

                limitDatas = orderDatas.ToList();
                if (resultLimit != null)
                {
                    if (limitDatas.Count > resultLimit)
                    {
                        //limitDatas = limitDatas.Take(resultLimit).ToList();
                        limitDatas = limitDatas.GetRange(0, resultLimit.Value);
                        isOverResultLimit = true;
                    }
                }
            }

            var data = new
            {
                Host = Request.Host.Value,
                Scheme = Request.Scheme,
                IsAndroid = Request.Headers.UserAgent
                    .ToString().Contains("Android"),
                HasSearch = hasSearch,
                //SearchText = search,
                IsOverResultLimit = isOverResultLimit,
                IsUseVariantSearch = _useVariantSearch,
                IsUseSearchAsync = _useSearchAsync,
                Datas = limitDatas,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);

            var encodeData = new
            {
                SearchText = search,
                SearchTextTW = _useVariantSearch ?
                    ChineseConverter.ToTraditional(search) : ""
            };
            EncodeData = System.Text.Json.JsonSerializer.Serialize(encodeData, _jsonOptions);
            var title = _localization.WorkDir;
            Title = !hasSearch ? title : $"{search} ({title})";
            if (hasSearch && _useVariantSearch)
                CachedChineseConverter.SaveCache();
            return Page();
        }
    }
}