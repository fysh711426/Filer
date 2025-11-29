using Filer.Extensions;
using Filer.Models;
using Filer.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Filer.Pages
{
    public class FolderModel : BasePageModel
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
                path = path?.Trim('/') ?? "";
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

            var datas = new List<FileModel>();

            var folders = null as IEnumerable<string>;
            folders = !hasSearch ?
                Directory.GetDirectories(folderPath) :
                Directory.EnumerateDirectories(folderPath, $"*{search}*", SearchOption.AllDirectories);
            folders = folders
                .Where(it => it.StartsWith(workDir))
                .Select(it => it.Replace(workDir, "") .Replace(@"\", "/"));

            if (orderBy == "autoDesc")
                folders = folders.Reverse();

            foreach (var item in folders)
            {
                try
                {
                    var file = new FileModel();
                    file.FileType = FileType.Folder;
                    file.WorkNum = workNum;
                    file.WorkDir = GetWorkDirName(workNum);
                    file.Path = item;
                    file.Name = Path.GetFileName(item);
                    var itemPath = Path.GetFullPath(Path.Combine(workDir, item));
                    var fileCount = Directory.GetFiles(itemPath).Length;
                    var folderCount = Directory.GetDirectories(itemPath).Length;
                    file.ItemCount = fileCount + folderCount;
                    var _pathInfo = GetPathInfo(item);
                    file.DirPath = _pathInfo.parentPath;
                    file.DirName = _pathInfo.parentName;
                    file.ParentDirPath = _pathInfo.grandParentPath;
                    file.ParentDirName = _pathInfo.grandParentName;

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
                        if (file.HistoryCount == fileCount + folderCount)
                            file.HasHistory = true;
                    }
                    datas.Add(file);
                }
                catch { }
            }

            var files = null as IEnumerable<string>;
            files = !hasSearch ?
                Directory.GetFiles(folderPath) :
                Directory.EnumerateFiles(folderPath, $"*{search}*", SearchOption.AllDirectories);
            files = files
                .Where(it => it.StartsWith(workDir))
                .Select(it => it.Replace(workDir, "").Replace(@"\", "/"));

            if (orderBy == "autoDesc")
                files = files.Reverse();

            foreach (var item in files)
            {
                var model = new FileModel();
                model.WorkNum = workNum;
                model.WorkDir = GetWorkDirName(workNum);
                model.Path = item;
                model.Name = Path.GetFileName(item);
                var filePath = Path.GetFullPath(Path.Combine(workDir, item));
                model.FileLength = new FileInfo(filePath).Length;
                model.FileSize = FormatFileSize(model.FileLength);
                model.LastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(filePath);
                model.LastWriteTimeUtcText = model.LastWriteTimeUtc.ToString("yyyy/MM/dd HH:mm:ss");
                var _pathInfo = GetPathInfo(item);
                model.DirPath = _pathInfo.parentPath;
                model.DirName = _pathInfo.parentName;
                model.ParentDirPath = _pathInfo.grandParentPath;
                model.ParentDirName = _pathInfo.grandParentName;

                if (_useHistory)
                {
                    var historyDir = GetAppDirectory("History");
                    var parentDir = Path.Combine($"{workNum}", Path.GetDirectoryName(item) ?? "");
                    if (!string.IsNullOrWhiteSpace(parentDir))
                    {
                        var historySubDir = Path.GetFullPath(Path.Combine(historyDir, parentDir));
                        if (historySubDir.StartsWith(historyDir))
                        {
                            var historyPath = Path.Combine(historySubDir, filePath.ToMD5());
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
                    datas.Add(model);
                    continue;
                }
                if (_videoMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Video;
                    model.MimeType = mimeType;
                    datas.Add(model);
                    continue;
                }
                if (_audioMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Audio;
                    model.MimeType = mimeType;
                    datas.Add(model);
                    continue;
                }
                if (_textMimeType.ContainsKey(mimeType))
                {
                    model.FileType = FileType.Text;
                    model.MimeType = mimeType;
                    datas.Add(model);
                    continue;
                }
                datas.Add(model);
            }

            // Update HistoryCount
            if (_useHistory)
            {
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
                        }

                        if (totalCount > 0 || totalCount != historyCount)
                        {
                            if (!Directory.Exists(historySubDir))
                                Directory.CreateDirectory(historySubDir);
                            System.IO.File.WriteAllText(historyPath, $"{totalCount}");
                        }
                    }
                }
            }

            var orderDatas = datas
                .OrderBy(it => it.FileType);

            if (orderBy == "name")
                orderDatas = !_useWindowsNaturalSort ?
                    orderDatas.ThenBy(it => it.Name) :
                    orderDatas.ThenBy(it => it.Name, new WindowsNaturalSort());
            else if (orderBy == "nameDesc")
                orderDatas = !_useWindowsNaturalSort ?
                    orderDatas.ThenByDescending(it => it.Name) :
                    orderDatas.ThenByDescending(it => it.Name, new WindowsNaturalSort());
            else if (orderBy == "date")
                orderDatas = orderDatas.ThenBy(it => it.LastWriteTimeUtc);
            else if (orderBy == "dateDesc")
                orderDatas = orderDatas.ThenByDescending(it => it.LastWriteTimeUtc);
            else if (orderBy == "size")
                orderDatas = orderDatas.ThenBy(it => it.FileLength);
            else if (orderBy == "sizeDesc")
                orderDatas = orderDatas.ThenByDescending(it => it.FileLength);
            else
            {
                // auto
            }
            
            datas = orderDatas.ToList();

            var data = new
            {
                Host = Request.Host.Value,
                Scheme = Request.Scheme,
                IsAndroid = Request.Headers.UserAgent
                    .ToString().Contains("Android"),
                WorkNum = workNum,
                WorkDir = GetWorkDirName(workNum),
                DirPath = dirPath,
                DirName = dirName,
                ParentDirPath = parentDirPath,
                ParentDirName = parentDirName,
                HasSearch = hasSearch,
                //SearchText = search,
                Datas = datas,
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