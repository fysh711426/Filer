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

            var resultLimit = hasSearch ? _searchResultLimit : null as int?;

            var workDirs = GetWorkDirs().ToList();

            var datas = workDirs.AsEnumerable();
            if (hasSearch)
                datas = datas.Where(it =>
                    it.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (hasSearch)
            {
                datas = datas.Concat(
                    GetWorkDirsFiles(search, hasSearch, resultLimit));
            }

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
                HasSearch = hasSearch,
                //SearchText = search,
                isOverResultLimit = isOverResultLimit,
                Datas = limitDatas,
                Local = _localization
            };
            Data = JsonConvert.SerializeObject(data, _jsonSettings);

            var encodeData = new
            {
                SearchText = search,
            };
            EncodeData = System.Text.Json.JsonSerializer.Serialize(encodeData, _jsonOptions);
            return Page();
        }
    }
}