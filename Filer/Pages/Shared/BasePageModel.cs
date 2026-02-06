using Filer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace Filer.Pages.Shared
{
    public class BasePageModel : PageModel
    {
        public string Title { get; set; } = "";
        public string? Data { get; set; } = null;
        public string? EncodeData { get; set; } = null;

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

        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IConfiguration _configuration;
        protected readonly bool _useHistory;
        protected readonly bool _useWindowsNaturalSort;
        protected readonly bool _useSearchAsync;
        protected readonly bool _useVariantSearch;
        protected readonly int _searchResultLimit;
        protected readonly string _language;
        protected readonly List<WorkDir> _workDirs;
        protected readonly Localization _localization;
        public BasePageModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _useHistory = _configuration.GetValue<bool>("UseHistory");
            _useWindowsNaturalSort = _configuration.GetValue<bool>("UseWindowsNaturalSort");
            _useSearchAsync = _configuration.GetValue<bool>("UseSearchAsync");
            _useVariantSearch = _configuration.GetValue<bool>("UseVariantSearch");
            _searchResultLimit = _configuration.GetValue<int?>("SearchResultLimit") ?? 1000;
            _language = _configuration.GetValue<string>("Language") ?? "";
            var workDirs = _configuration
                .GetSection("WorkDirs").Get<List<WorkDir>>()
                ?? new List<WorkDir>();
            var index = 0;
            foreach(var item in workDirs)
            {
                item.Path = item.Path.TrimEnd(Path.DirectorySeparatorChar);
                item.Path = $@"{item.Path}{Path.DirectorySeparatorChar}";
                item.IsPathError = !Directory.Exists(item.Path);
                item.Index = index++;
            }
            _workDirs = workDirs;
            _localization = GetLocalization(_configuration, _language);
        }

        protected string GetWorkDirName(int workNum)
        {
            return _workDirs[workNum - 1].Name;
        }

        //----- 舊寫法 -----
        //[Obsolete]
        //protected (string parentPath, string parentName, string path, string pathName)
        //    GetPathInfo(int workNum, string path)
        //{
        //    var parentPath = Path
        //        .GetDirectoryName(path)?.Replace(@"\", "/") ?? "";
        //    return (
        //        parentPath, GetPathName(workNum, parentPath),
        //        path, GetPathName(workNum, path)
        //    );
        //}

        //[Obsolete]
        //private string GetPathName(int workNum, string path)
        //{
        //    if (path == "")
        //        return _workDirs[workNum - 1].Name;
        //    return Path.GetFileName(path);
        //}
        //----- 舊寫法 -----

        private Localization GetLocalization(IConfiguration configuration, string language)
        {
            return new Localization
            {
                WorkDir = configuration.GetValue<string>($"Localization:{language}:WorkDir") ?? "",
                WorkDirNotSet = configuration.GetValue<string>($"Localization:{language}:WorkDirNotSet") ?? "",
                PathNotFound = configuration.GetValue<string>($"Localization:{language}:PathNotFound") ?? "",
                Items = configuration.GetValue<string>($"Localization:{language}:Items") ?? "",
                Search = configuration.GetValue<string>($"Localization:{language}:Search") ?? "",
                SearchResult = configuration.GetValue<string>($"Localization:{language}:SearchResult") ?? "",
                Browse = configuration.GetValue<string>($"Localization:{language}:Browse") ?? "",
                Thumbnail = configuration.GetValue<string>($"Localization:{language}:Thumbnail") ?? "",
                Auto = configuration.GetValue<string>($"Localization:{language}:Auto") ?? "",
                Name = configuration.GetValue<string>($"Localization:{language}:Name") ?? "",
                Date = configuration.GetValue<string>($"Localization:{language}:Date") ?? "",
                Size = configuration.GetValue<string>($"Localization:{language}:Size") ?? "",
                Settings = configuration.GetValue<string>($"Localization:{language}:Settings") ?? "",
                Version = configuration.GetValue<string>($"Localization:{language}:Version") ?? "",
                Enable = configuration.GetValue<string>($"Localization:{language}:Enable") ?? "",
                SaveSuccess = configuration.GetValue<string>($"Localization:{language}:SaveSuccess") ?? "",
                UseDeepLinkDescription = configuration.GetValue<string>($"Localization:{language}:UseDeepLinkDescription") ?? "",
                DeepLinkPackageDescriptionFirst = configuration.GetValue<string>($"Localization:{language}:DeepLinkPackageDescriptionFirst") ?? "",
                DeepLinkPackageDescriptionSecond = configuration.GetValue<string>($"Localization:{language}:DeepLinkPackageDescriptionSecond") ?? "",
                SearchInputErrorMessage = configuration.GetValue<string>($"Localization:{language}:SearchInputErrorMessage") ?? "",
                OverResultLimitErrorMessage = configuration.GetValue<string>($"Localization:{language}:OverResultLimitErrorMessage") ?? "",
                Bookmark = configuration.GetValue<string>($"Localization:{language}:Bookmark") ?? "",
                BookmarkSave = configuration.GetValue<string>($"Localization:{language}:BookmarkSave") ?? "",
                BookmarkRemove = configuration.GetValue<string>($"Localization:{language}:BookmarkRemove") ?? "",
                BookmarkSaveMessage = configuration.GetValue<string>($"Localization:{language}:BookmarkSaveMessage") ?? "",
                BookmarkRemoveMessage = configuration.GetValue<string>($"Localization:{language}:BookmarkRemoveMessage") ?? "",
                Group = configuration.GetValue<string>($"Localization:{language}:Group") ?? "",
                CreateGroup = configuration.GetValue<string>($"Localization:{language}:CreateGroup") ?? "",
                EditGroup = configuration.GetValue<string>($"Localization:{language}:EditGroup") ?? "",
                DeleteGroupConfirmMessage = configuration.GetValue<string>($"Localization:{language}:DeleteGroupConfirmMessage") ?? "",
                DeleteBookmarkConfirmMessage = configuration.GetValue<string>($"Localization:{language}:DeleteBookmarkConfirmMessage") ?? "",
                GroupNameExists = configuration.GetValue<string>($"Localization:{language}:GroupNameExists") ?? "",
                Create = configuration.GetValue<string>($"Localization:{language}:Create") ?? "",
                Edit = configuration.GetValue<string>($"Localization:{language}:Edit") ?? "",
                Delete = configuration.GetValue<string>($"Localization:{language}:Delete") ?? "",
                Import = configuration.GetValue<string>($"Localization:{language}:Import") ?? "",
                Export = configuration.GetValue<string>($"Localization:{language}:Export") ?? "",
                Upload = configuration.GetValue<string>($"Localization:{language}:Upload") ?? "",
                Download = configuration.GetValue<string>($"Localization:{language}:Download") ?? "",
                Confirm = configuration.GetValue<string>($"Localization:{language}:Confirm") ?? "",
                Cancel = configuration.GetValue<string>($"Localization:{language}:Cancel") ?? "",
                Sync = configuration.GetValue<string>($"Localization:{language}:Sync") ?? "",
                SyncBookmark = configuration.GetValue<string>($"Localization:{language}:SyncBookmark") ?? "",
                SyncModalTitle = configuration.GetValue<string>($"Localization:{language}:SyncModalTitle") ?? "",
                SyncModalContent = configuration.GetValue<string>($"Localization:{language}:SyncModalContent") ?? "",
                SyncModalAlertText = configuration.GetValue<string>($"Localization:{language}:SyncModalAlertText") ?? "",
                Backup = configuration.GetValue<string>($"Localization:{language}:Backup") ?? "",
                BackupBookmark = configuration.GetValue<string>($"Localization:{language}:BackupBookmark") ?? "",
                BackupModalTitle = configuration.GetValue<string>($"Localization:{language}:BackupModalTitle") ?? "",
                BackupModalContent = configuration.GetValue<string>($"Localization:{language}:BackupModalContent") ?? "",
                BackupModalAlertText = configuration.GetValue<string>($"Localization:{language}:BackupModalAlertText") ?? "",
                BackupEmpty = configuration.GetValue<string>($"Localization:{language}:BackupEmpty") ?? "",
                ImportBookmark = configuration.GetValue<string>($"Localization:{language}:ImportBookmark") ?? "",
                ImportModalTitle = configuration.GetValue<string>($"Localization:{language}:ImportModalTitle") ?? "",
                ImportModalContent = configuration.GetValue<string>($"Localization:{language}:ImportModalContent") ?? "",
                ImportModalAlertText = configuration.GetValue<string>($"Localization:{language}:ImportModalAlertText") ?? "",
                ImportModalCheckboxText = configuration.GetValue<string>($"Localization:{language}:ImportModalCheckboxText") ?? "",
                ChooseFile = configuration.GetValue<string>($"Localization:{language}:ChooseFile") ?? "",
                ClickToChooseFile = configuration.GetValue<string>($"Localization:{language}:ClickToChooseFile") ?? "",
                Selected = configuration.GetValue<string>($"Localization:{language}:Selected") ?? "",
                Yes = configuration.GetValue<string>($"Localization:{language}:Yes") ?? "",
                No = configuration.GetValue<string>($"Localization:{language}:No") ?? "",
                Loading = configuration.GetValue<string>($"Localization:{language}:Loading") ?? "",
                Uploading = configuration.GetValue<string>($"Localization:{language}:Uploading") ?? "",
                Syncing = configuration.GetValue<string>($"Localization:{language}:Syncing") ?? "",
            };
        }
    }
}