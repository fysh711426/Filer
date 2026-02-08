using Filer.Extensions;
using Filer.Models;
using MimeTypes;
using static Filer.Extensions.Helper;
using static Filer.Extensions.MimeTypeHelper;
using static Filer.Extensions.PathHelper;

namespace Filer.Services
{
    public class FolderService
    {
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

        protected readonly bool _useHistory;
        protected readonly bool _useWindowsNaturalSort;
        protected readonly bool _useVariantSearch;
        protected readonly List<WorkDir> _workDirs;

        public FolderService(
            bool useHistory,
            bool useWindowsNaturalSort,
            bool useVariantSearch,
            List<WorkDir> workDirs)
        {
            _useHistory = useHistory;
            _useWindowsNaturalSort = useWindowsNaturalSort;
            _useVariantSearch = useVariantSearch;
            _workDirs = workDirs;
        }

        protected string GetWorkDirName(int workNum)
        {
            return _workDirs[workNum - 1].Name;
        }

        public IEnumerable<FileModel> GetWorkDirsFiles(string search, bool hasSearch, int? resultLimit = null)
        {
            var datas = EnumerateWorkDirsFiles(search, hasSearch);
            if (resultLimit != null)
                datas = datas.Take(resultLimit.Value + 1);
            return datas;
        }

        protected IEnumerable<FileModel> EnumerateWorkDirsFiles(string search, bool hasSearch)
        {
            var pathDict = new HashSet<string>();

            foreach (var item in _workDirs)
            {
                if (!item.IsPathError)
                {
                    var path = "";
                    var workNum = item.Index + 1;
                    var workDir = item.Path;
                    var folderPath = Path.GetFullPath(Path.Combine(workDir, path));

                    if (Directory.Exists(folderPath))
                    {
                        if (folderPath.StartsWith(workDir))
                        {
                            var files = GetFiles(
                                workNum, workDir, path, folderPath, search, hasSearch);
                            foreach (var file in files)
                            {
                                if (!pathDict.Add(
                                    Path.GetFullPath(Path.Combine(workDir, file.Path))))
                                    continue;
                                yield return file;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<FileModel> GetFiles(
            int workNum, string workDir, string path, string folderPath, string search, bool hasSearch, int? resultLimit = null)
        {
            //var folders = EnumerateFolders(workNum, workDir, folderPath, search, hasSearch);

            //var files = EnumerateFiles(workNum, workDir, folderPath, search, hasSearch);

            //var datas = folders.Concat(files);

            var datas = EnumerateEntries(workNum, workDir, folderPath, search, hasSearch);

            if (resultLimit != null)
                datas = datas.Take(resultLimit.Value + 1);
            return datas;
        }

        public FileModel CreateFolder(int workNum, string workDir, bool hasSearch, int index, string item)
        {
            var model = new FileModel();
            model.FileType = FileType.Folder;
            model.WorkNum = workNum;
            model.WorkDir = GetWorkDirName(workNum);
            model.Path = item;
            model.Name = Path.GetFileName(model.Path);
            var fullPath = Path.GetFullPath(Path.Combine(workDir, item));
            //var info = new DirectoryInfo(fullPath);
            //file.ItemCount = Directory.GetFileSystemEntries(fullPath).Length;
            model.ItemCount = Directory.GetFileSystemEntries(fullPath, "*", _enumerationOptions).Length;
            var _pathInfo = GetPathInfo(item);
            model.DirPath = _pathInfo.parentPath;
            model.DirName = _pathInfo.parentName;
            model.ParentDirPath = _pathInfo.grandParentPath;
            model.ParentDirName = _pathInfo.grandParentName;
            //model.Index = index++;
            model.Index = index;

            if (hasSearch && _useVariantSearch)
                model.NameTW = CachedChineseConverter.ToTraditional(model.Name);

            if (_useHistory)
            {
                var historyDir = GetAppDirectory("History");
                var folderDir = Path.Combine($"{workNum}", item);
                var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                if (historySubDir.StartsWith(historyDir))
                {
                    var historyPath = Path.Combine(historySubDir, "HistoryCount");
                    if (File.Exists(historyPath))
                    {
                        var countText = File.ReadAllText(historyPath);
                        if (int.TryParse(countText, out var count))
                            model.HistoryCount = count;
                    }
                }
                if (model.HistoryCount == model.ItemCount)
                    model.HasHistory = true;
            }
            return model;
        }

        public FileModel CreateFile(int workNum, string workDir, bool hasSearch, int index, string item)
        {
            var model = new FileModel();
            model.WorkNum = workNum;
            model.WorkDir = GetWorkDirName(workNum);
            model.Path = item;
            model.Name = Path.GetFileName(model.Path);
            var fullPath = Path.GetFullPath(Path.Combine(workDir, item));
            var info = new FileInfo(fullPath);
            model.FileLength = info.Length;
            model.FileSize = FormatFileSize(model.FileLength);
            model.LastWriteTimeUtc = info.LastWriteTimeUtc;
            model.LastWriteTimeUtcText = model.LastWriteTimeUtc.ToString("yyyy/MM/dd HH:mm:ss");
            var _pathInfo = GetPathInfo(item);
            model.DirPath = _pathInfo.parentPath;
            model.DirName = _pathInfo.parentName;
            model.ParentDirPath = _pathInfo.grandParentPath;
            model.ParentDirName = _pathInfo.grandParentName;
            //model.Index = index++;
            model.Index = index;

            if (hasSearch && _useVariantSearch)
                model.NameTW = CachedChineseConverter.ToTraditional(model.Name);

            if (_useHistory)
            {
                var historyDir = GetAppDirectory("History");
                var parentDir = Path.Combine($"{workNum}", Path.GetDirectoryName(item) ?? "");
                var historySubDir = Path.GetFullPath(Path.Combine(historyDir, parentDir));
                if (historySubDir.StartsWith(historyDir))
                {
                    var historyPath = Path.Combine(historySubDir, fullPath.ToMD5());
                    if (File.Exists(historyPath))
                        model.HasHistory = true;
                }
            }

            var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
            if (ImageMimeType.ContainsKey(mimeType))
            {
                model.FileType = FileType.Image;
                model.MimeType = mimeType;
                //var img = Image.Identify(filePath);
                //model.Width = img.Width;
                //model.Height = img.Height;
            }
            else if (VideoMimeType.ContainsKey(mimeType))
            {
                model.FileType = FileType.Video;
                model.MimeType = mimeType;
            }
            else if (AudioMimeType.ContainsKey(mimeType))
            {
                model.FileType = FileType.Audio;
                model.MimeType = mimeType;
            }
            else if (TextMimeType.ContainsKey(mimeType))
            {
                model.FileType = FileType.Text;
                model.MimeType = mimeType;
            }
            return model;
        }

        public IEnumerable<FileModel> EnumerateFolders(
            int workNum, string workDir, string folderPath, string search = "", bool hasSearch = false, int? resultLimit = null)
        {
            var folders = GetEnumerateFolders(folderPath, search, hasSearch)
                //.DebugEnumerable("Folders")
                .Where(it => it.StartsWith(workDir))
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));

            if (resultLimit != null)
                folders = folders.Take(resultLimit.Value + 1);

            //if (orderBy == "autoDesc")
            //    folders = folders.Reverse();

            var index = 0;
            foreach (var item in folders)
            {
                yield return CreateFolder(
                    workNum, workDir, hasSearch, index++, item);
            }
        }

        public IEnumerable<FileModel> EnumerateFiles(
            int workNum, string workDir, string folderPath, string search = "", bool hasSearch = false, int? resultLimit = null)
        {
            var files = GetEnumerateFiles(folderPath, search, hasSearch)
                //.DebugEnumerable("Files")
                .Where(it => it.StartsWith(workDir))
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));

            if (resultLimit != null)
                files = files.Take(resultLimit.Value + 1);

            //if (orderBy == "autoDesc")
            //    files = files.Reverse();

            var index = 0;
            foreach (var item in files)
            {
                yield return CreateFile(
                    workNum, workDir, hasSearch, index++, item);
            }
        }

        public IEnumerable<FileModel> EnumerateEntries(
            int workNum, string workDir, string folderPath, string search = "", bool hasSearch = false, int? resultLimit = null)
        {
            var entries = GetEnumerateEntries(folderPath, search, hasSearch)
                //.DebugEnumerable("Entries")
                .Where(it => it.StartsWith(workDir))
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));

            if (resultLimit != null)
                entries = entries.Take(resultLimit.Value + 1);

            //if (orderBy == "autoDesc")
            //    entries = entries.Reverse();

            var index = 0;
            foreach (var item in entries)
            {
                var info = new FileInfo(
                    Path.GetFullPath(Path.Combine(workDir, item)));
                if (info.Attributes.HasFlag(FileAttributes.Directory))
                    yield return CreateFolder(
                        workNum, workDir, hasSearch, index++, item);
                else
                    yield return CreateFile(
                        workNum, workDir, hasSearch, index++, item);
            }
        }

        public IEnumerable<string> GetEnumerateFolders(string folderPath, string search = "", bool hasSearch = false)
        {
            if (!hasSearch)
                return Directory.EnumerateDirectories(folderPath, "*", _enumerationOptions);

            if (!_useVariantSearch)
                return Directory.EnumerateDirectories(folderPath, $"*{search}*", _enumerationRecursiveOptions);

            var searchText = ChineseConverter.ToTraditional(search);
            return Directory.EnumerateDirectories(folderPath, "*", _enumerationRecursiveOptions)
                .Where(it => CachedChineseConverter.ToTraditional(Path.GetFileName(it))
                    .Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> GetEnumerateFiles(string folderPath, string search = "", bool hasSearch = false)
        {
            if (!hasSearch)
                return Directory.EnumerateFiles(folderPath, "*", _enumerationOptions);

            if (!_useVariantSearch)
                return Directory.EnumerateFiles(folderPath, $"*{search}*", _enumerationRecursiveOptions);

            var searchText = ChineseConverter.ToTraditional(search);
            return Directory.EnumerateFiles(folderPath, "*", _enumerationRecursiveOptions)
                .Where(it => CachedChineseConverter.ToTraditional(Path.GetFileName(it))
                    .Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> GetEnumerateEntries(string folderPath, string search = "", bool hasSearch = false)
        {
            if (!hasSearch)
                return Directory.EnumerateFileSystemEntries(folderPath, "*", _enumerationOptions);

            if (!_useVariantSearch)
                return Directory.EnumerateFileSystemEntries(folderPath, $"*{search}*", _enumerationRecursiveOptions);

            var searchText = ChineseConverter.ToTraditional(search);
            return Directory.EnumerateFileSystemEntries(folderPath, "*", _enumerationRecursiveOptions)
                .Where(it => CachedChineseConverter.ToTraditional(Path.GetFileName(it))
                    .Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public IOrderedEnumerable<FileModel> OrderBy(IEnumerable<FileModel> datas, string? orderBy, bool hasSearch = false)
        {
            var orderDatas = datas
                .OrderBy(it => it.FileType);

            var useNatural = _useWindowsNaturalSort;

            if (orderBy == "name")
                orderDatas = orderDatas
                    .ThenByNatural(it => it.Name, useNatural)
                    .ThenBy(it => it.Index);
            else if (orderBy == "nameDesc")
                orderDatas = orderDatas
                    .ThenByNaturalDescending(it => it.Name, useNatural)
                    .ThenByDescending(it => it.Index);
            else if (orderBy == "date")
                orderDatas = orderDatas
                    .ThenBy(it => it.LastWriteTimeUtc)
                    .ThenByNatural(it => it.Name, useNatural)
                    .ThenBy(it => it.Index);
            else if (orderBy == "dateDesc")
                orderDatas = orderDatas
                    .ThenByDescending(it => it.LastWriteTimeUtc)
                    .ThenByNaturalDescending(it => it.Name, useNatural)
                    .ThenByDescending(it => it.Index);
            else if (orderBy == "size")
                orderDatas = orderDatas
                    .ThenBy(it => it.FileLength)
                    .ThenByNatural(it => it.Name, useNatural)
                    .ThenBy(it => it.Index);
            else if (orderBy == "sizeDesc")
                orderDatas = orderDatas
                    .ThenByDescending(it => it.FileLength)
                    .ThenByNaturalDescending(it => it.Name, useNatural)
                    .ThenByDescending(it => it.Index);
            else if (orderBy == "autoDesc")
                orderDatas = !hasSearch ?
                    orderDatas.ThenByDescending(it => it.Index) :
                    orderDatas.ThenByNaturalDescending(it => GetDirNameTitle(it.DirName, it.WorkDir), useNatural)
                              .ThenByNaturalDescending(it => it.Name, useNatural)
                              .ThenByDescending(it => it.Index);
            else   //auto
                orderDatas = !hasSearch ?
                    orderDatas.ThenBy(it => it.Index) :
                    orderDatas.ThenByNatural(it => GetDirNameTitle(it.DirName, it.WorkDir), useNatural)
                              .ThenByNatural(it => it.Name, useNatural)
                              .ThenBy(it => it.Index);
            return orderDatas;
        }

        public IEnumerable<FileModel> GetWorkDirs()
        {
            foreach (var item in _workDirs)
            {
                yield return CreateWorkDir(item);
            }
        }

        public FileModel CreateWorkDir(WorkDir item)
        {
            var model = new FileModel();
            model.FileType = FileType.WorkDir;
            model.Name = item.Name;
            model.WorkDir = item.Name;
            model.IsPathError = item.IsPathError;
            model.Index = item.Index;
            model.WorkNum = item.Index + 1;
            model.Path = "";

            if (!item.IsPathError)
            {
                var fullPath = Path.GetFullPath(item.Path);
                //var fileCount = Directory.GetFiles(itemPath).Length;
                //var folderCount = Directory.GetDirectories(itemPath).Length;
                //model.ItemCount = fileCount + folderCount;
                model.ItemCount = Directory.GetFileSystemEntries(fullPath, "*", _enumerationOptions).Length;

                if (_useHistory)
                {
                    var historyDir = GetAppDirectory("History");
                    var folderDir = $"{model.WorkNum}";
                    var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                    if (historySubDir.StartsWith(historyDir))
                    {
                        var historyPath = Path.Combine(historySubDir, "HistoryCount");
                        if (File.Exists(historyPath))
                        {
                            var countText = File.ReadAllText(historyPath);
                            if (int.TryParse(countText, out var count))
                                model.HistoryCount = count;
                        }
                    }
                    if (model.HistoryCount == model.ItemCount)
                        model.HasHistory = true;
                }
            }
            return model;
        }

        // 注意: 呼叫此函數後，不可截斷列表，不然 HistoryCount 不會更新
        // 例如: 呼叫 .Take(10) 或 .First()
        public IEnumerable<FileModel> UpdateHistoryCount(int workNum, string path, IEnumerable<FileModel> datas)
        {
            if (_useHistory)
            {
                // Update HistoryCount
                var historyDir = GetAppDirectory("History");
                var folderDir = Path.Combine($"{workNum}", path ?? "");
                if (!string.IsNullOrWhiteSpace(folderDir))
                {
                    var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                    if (historySubDir.StartsWith(historyDir))
                    {
                        var historyCount = 0;
                        var historyPath = Path.Combine(historySubDir, "HistoryCount");
                        if (File.Exists(historyPath))
                        {
                            var countText = File.ReadAllText(historyPath);
                            if (int.TryParse(countText, out var count))
                                historyCount = count;
                        }

                        var totalCount = 0;
                        foreach (var item in datas)
                        {
                            //if (item.HasHistory)
                            //{
                            //    if (item.FileType == FileType.Folder)
                            //        totalCount += item.HistoryCount;
                            //    else
                            //        totalCount++;
                            //}
                            if (item.HasHistory)
                                totalCount++;
                            yield return item;
                        }

                        if (totalCount > 0 && totalCount != historyCount)
                        {
                            if (!Directory.Exists(historySubDir))
                                Directory.CreateDirectory(historySubDir);
                            File.WriteAllText(historyPath, $"{totalCount}");
                        }
                        yield break;
                    }
                }
            }
            foreach (var item in datas)
                yield return item;
        }
    }
}