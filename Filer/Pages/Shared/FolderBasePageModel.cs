using Filer.Extensions;
using Filer.Models;
using MimeTypes;

namespace Filer.Pages.Shared
{
    public class FolderBasePageModel : BasePageModel
    {
        public FolderBasePageModel(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
        }

        protected IEnumerable<FileModel> GetWorkDirsFiles(string search, bool hasSearch, int? resultLimit = null)
        {
            var datas = EnumerateWorkDirsFiles(search, hasSearch);
            if (resultLimit != null)
                datas = datas.Take(resultLimit.Value + 1);
            return datas;
        }

        private IEnumerable<FileModel> EnumerateWorkDirsFiles(string search, bool hasSearch)
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

        protected IEnumerable<FileModel> GetFiles(
            int workNum, string workDir, string path, string folderPath, string search, bool hasSearch, int? resultLimit = null)
        {
            var folders = EnumerateFolders(workNum, workDir, folderPath, search, hasSearch);

            var files = EnumerateFiles(workNum, workDir, folderPath, search, hasSearch);

            var datas = folders.Concat(files);

            if (resultLimit != null)
                datas = datas.Take(resultLimit.Value + 1);
            return datas;
        }

        protected IEnumerable<FileModel> EnumerateFolders(
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
                var file = new FileModel();
                file.FileType = FileType.Folder;
                file.WorkNum = workNum;
                file.WorkDir = GetWorkDirName(workNum);
                file.Path = item;
                file.Name = Path.GetFileName(item);
                var fullPath = Path.GetFullPath(Path.Combine(workDir, item));
                //var fileCount = Directory.GetFiles(fullPath).Length;
                //var folderCount = Directory.GetDirectories(fullPath).Length;
                //file.ItemCount = fileCount + folderCount;
                file.ItemCount = Directory.GetFileSystemEntries(fullPath).Length;
                var _pathInfo = GetPathInfo(item);
                file.DirPath = _pathInfo.parentPath;
                file.DirName = _pathInfo.parentName;
                file.ParentDirPath = _pathInfo.grandParentPath;
                file.ParentDirName = _pathInfo.grandParentName;
                file.Index = index++;

                if (_useHistory)
                {
                    var historyDir = GetAppDirectory("History");
                    var folderDir = Path.Combine($"{workNum}", item ?? "");
                    if (!string.IsNullOrWhiteSpace(folderDir))
                    {
                        var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                        if (historySubDir.StartsWith(historyDir))
                        {
                            var historyPath = Path.Combine(historySubDir, "HistoryCount");
                            if (System.IO.File.Exists(historyPath))
                            {
                                var countText = System.IO.File.ReadAllText(historyPath);
                                if (int.TryParse(countText, out var count))
                                    file.HistoryCount = count;
                            }
                        }
                    }
                    if (file.HistoryCount == file.ItemCount)
                        file.HasHistory = true;
                }
                yield return file;
            }
        }

        protected IEnumerable<FileModel> EnumerateFiles(
            int workNum, string workDir, string folderPath, string search = "", bool hasSearch = false, int? resultLimit = null)
        {
            var files = GetEnumerateFiles(folderPath, search, hasSearch)
                //.DebugEnumerable("Files")
                .Where(it => it.FullName.StartsWith(workDir));

            if (resultLimit != null)
                files = files.Take(resultLimit.Value + 1);

            //if (orderBy == "autoDesc")
            //    files = files.Reverse();

            var index = 0;
            foreach (var info in files)
            {
                var item = info.FullName
                    .Replace(workDir, "").Replace(@"\", "/");

                var model = new FileModel();
                model.WorkNum = workNum;
                model.WorkDir = GetWorkDirName(workNum);
                model.Path = item;
                model.Name = Path.GetFileName(model.Path);
                var fullPath = info.FullName;
                model.FileLength = info.Length;
                model.FileSize = FormatFileSize(model.FileLength);
                model.LastWriteTimeUtc = info.LastWriteTimeUtc;
                model.LastWriteTimeUtcText = model.LastWriteTimeUtc.ToString("yyyy/MM/dd HH:mm:ss");
                var _pathInfo = GetPathInfo(item);
                model.DirPath = _pathInfo.parentPath;
                model.DirName = _pathInfo.parentName;
                model.ParentDirPath = _pathInfo.grandParentPath;
                model.ParentDirName = _pathInfo.grandParentName;
                model.Index = index++;

                if (_useHistory)
                {
                    var historyDir = GetAppDirectory("History");
                    var parentDir = Path.Combine($"{workNum}", Path.GetDirectoryName(item) ?? "");
                    if (!string.IsNullOrWhiteSpace(parentDir))
                    {
                        var historySubDir = Path.GetFullPath(Path.Combine(historyDir, parentDir));
                        if (historySubDir.StartsWith(historyDir))
                        {
                            var historyPath = Path.Combine(historySubDir, fullPath.ToMD5());
                            if (System.IO.File.Exists(historyPath))
                                model.HasHistory = true;
                        }
                    }
                }

                var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(item));
                if (_imageMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Image;
                    model.MimeType = mimeType;
                    //var img = Image.Identify(filePath);
                    //model.Width = img.Width;
                    //model.Height = img.Height;
                }
                else if (_videoMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Video;
                    model.MimeType = mimeType;
                }
                else if (_audioMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Audio;
                    model.MimeType = mimeType;
                }
                else if (_textMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Text;
                    model.MimeType = mimeType;
                }
                yield return model;
            }
        }

        protected IEnumerable<string> GetEnumerateFolders(string folderPath, string search = "", bool hasSearch = false)
        {
            if (!hasSearch)
                return Directory.EnumerateDirectories(folderPath);

            if (!_enableChineseVariantSearch)
                return Directory.EnumerateDirectories(folderPath, $"*{search}*", SearchOption.AllDirectories);

            var searchText = ChineseConverter.ToTraditional(search);
            return Directory.EnumerateDirectories(folderPath, $"*", SearchOption.AllDirectories)
                .Where(it => ChineseConverter.ToTraditional(Path.GetFileName(it))
                    .Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        protected IEnumerable<FileInfo> GetEnumerateFiles(string folderPath, string search = "", bool hasSearch = false)
        {
            if (!hasSearch)
                return new DirectoryInfo(folderPath).EnumerateFiles();

            if (!_enableChineseVariantSearch)
                return new DirectoryInfo(folderPath).EnumerateFiles($"*{search}*", SearchOption.AllDirectories);

            var searchText = ChineseConverter.ToTraditional(search);
            return new DirectoryInfo(folderPath).EnumerateFiles($"*", SearchOption.AllDirectories)
                .Where(it => ChineseConverter.ToTraditional(it.Name)
                    .Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        protected IOrderedEnumerable<FileModel> OrderBy(IEnumerable<FileModel> datas, string? orderBy, bool hasSearch = false)
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

        protected string GetDirNameTitle(string dirName, string workDir)
        {
            if (!string.IsNullOrWhiteSpace(dirName))
                return dirName;
            if (!string.IsNullOrWhiteSpace(workDir))
                return workDir;
            return "";
        }

        protected IEnumerable<FileModel> GetWorkDirs()
        {
            foreach (var item in _workDirs)
            {
                var file = new FileModel();
                file.FileType = FileType.WorkDir;
                file.Name = item.Name;
                file.WorkDir = item.Name;
                file.IsPathError = item.IsPathError;
                file.Index = item.Index;
                file.WorkNum = item.Index + 1;
                file.Path = "";

                if (!item.IsPathError)
                {
                    var itemPath = Path.GetFullPath(item.Path);
                    var fileCount = Directory.GetFiles(itemPath).Length;
                    var folderCount = Directory.GetDirectories(itemPath).Length;
                    file.ItemCount = fileCount + folderCount;

                    if (_useHistory)
                    {
                        var historyDir = GetAppDirectory("History");
                        var folderDir = $"{file.WorkNum}";
                        var historySubDir = Path.GetFullPath(Path.Combine(historyDir, folderDir));
                        if (historySubDir.StartsWith(historyDir))
                        {
                            var historyPath = Path.Combine(historySubDir, "HistoryCount");
                            if (System.IO.File.Exists(historyPath))
                            {
                                var countText = System.IO.File.ReadAllText(historyPath);
                                if (int.TryParse(countText, out var count))
                                    file.HistoryCount = count;
                            }
                        }
                        if (file.HistoryCount == fileCount + folderCount)
                            file.HasHistory = true;
                    }
                }
                yield return file;
            }
        }

        // 注意: 呼叫此函數後，不可截斷列表，不然 HistoryCount 不會更新
        // 例如: 呼叫 .Take(10) 或 .First()
        protected IEnumerable<FileModel> UpdateHistoryCount(
            int workNum, string path, IEnumerable<FileModel> datas)
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
                        if (System.IO.File.Exists(historyPath))
                        {
                            var countText = System.IO.File.ReadAllText(historyPath);
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
                            System.IO.File.WriteAllText(historyPath, $"{totalCount}");
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