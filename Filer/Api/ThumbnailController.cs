using Filer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Filer.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThumbnailController : BaseController
    {
        public ThumbnailController(IConfiguration configuration)
            : base(configuration)
        {
        }

        [HttpGet("video/{worknum}/{*path}")]
        public async Task<IActionResult> _Video(int worknum, string path)
        {
            var scale = "480:360";

            var workDir = "";
            var filePath = "";
            try
            {
                workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
            }
            catch
            {
                return NotFound();
            }

            var fileInfo = new FileInfo(filePath);
            var lastModified = fileInfo.LastWriteTimeUtc;
            //var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);
            var fileSize = fileInfo.Length;

            var md5 = "";
            var thumbnailPath = "";
            if (_useThumbnailCache)
            {
                var thumbnailDir = GetAppDirectory("Thumbnail");
                var parentDir = Path.Combine($"{worknum}", Path.GetDirectoryName(path) ?? "");
                if (string.IsNullOrWhiteSpace(parentDir))
                    throw new Exception("ThumbnailPath is not found parentDir.");

                var thumbnailSubDir = Path.GetFullPath(Path.Combine(thumbnailDir, parentDir));
                if (!thumbnailSubDir.StartsWith(thumbnailDir))
                    throw new Exception("ThumbnailPath is outside of the thumbnailDir.");

                if (!Directory.Exists(thumbnailSubDir))
                    Directory.CreateDirectory(thumbnailSubDir);

                var meta = new
                {
                    scale = scale,
                    fileSize = fileSize,
                    lastWriteTime = stringSegment.ToString(),
                    filePath = filePath,
                    workDir = workDir,
                    worknum = worknum
                };
                md5 = JsonConvert.SerializeObject(meta).ToMD5();
                thumbnailPath = Path.Combine(thumbnailSubDir, $"{md5}_thumbnail");
                if (System.IO.File.Exists(thumbnailPath))
                {
                    var fs = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);
                    //return File(fs, "image/png");
                    return File(fs, "image/png",
                        new DateTimeOffset(lastModified), entityTag);
                }
            }

            var stream = new MemoryStream();
            var arguments = $@"-ss 00:00:01.00 -i ""{filePath}"" -vf ""scale={scale}:force_original_aspect_ratio=decrease"" -vframes 1 -c:v png -f image2 pipe: -loglevel error";
            var info = new ProcessStartInfo("ffmpeg", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            using (var process = Process.Start(info))
            {
                if (process == null)
                    throw new Exception("Failed to start ffmpeg.");
                var task = process.StandardOutput.BaseStream.CopyToAsync(stream);
                if (!process.WaitForExit(TimeSpan.FromSeconds(30)))
                {
                    process.Kill();
                    throw new Exception("FFmpeg process timeout.");
                }
                await task;
                if (process.ExitCode != 0)
                    throw new Exception($"FFmpeg failed.");
            }
            stream.Position = 0;

            if (!_useThumbnailCache)
                //return File(process.StandardOutput.BaseStream, "image/png");
                return File(stream, "image/png",
                    new DateTimeOffset(lastModified), entityTag);

            var tempDir = GetAppDirectory("Temp");
            var tempPath = Path.Combine(tempDir, $"{md5}_thumbnail");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            //var success = true;
            //try
            //{
            //    using (var fs = System.IO.File.Create(tempPath))
            //    {
            //        process.StandardOutput.BaseStream.CopyTo(fs);
            //    }
            //    await process.WaitForExitAsync();
            //    if (process.ExitCode != 0)
            //        success = false;
            //    process.Dispose();

            //    if (!success)
            //        throw new Exception($"FFmpeg failed.");

            //    using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
            //    {
            //        Response.ContentType = "image/png";
            //        await WriteToBody(fs);
            //        return new EmptyResult();
            //    }
            //}
            //finally
            //{
            //    if (success)
            //    {
            //        if (System.IO.File.Exists(tempPath))
            //            System.IO.File.Move(tempPath, thumbnailPath);
            //    }
            //    else
            //    {
            //        if (System.IO.File.Exists(tempPath))
            //            System.IO.File.Delete(tempPath);
            //    }
            //}

            try
            {
                using (var _fs = System.IO.File.Create(tempPath))
                {
                    stream.CopyTo(_fs);
                }
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Move(tempPath, thumbnailPath);

                var fs = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);
                return File(fs, "image/png",
                    new DateTimeOffset(lastModified), entityTag);
            }
            catch
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
                return StatusCode(500, "Thumbnail processing error.");
            }
        }

        [HttpGet("video/preview/{worknum}/{*path}")]
        public async Task<IActionResult> VideoPreview(int worknum, string path)
        {
            var fps = 20;
            var split = 9;
            var scale = "480:360";

            var workDir = "";
            var filePath = "";
            try
            {
                workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
            }
            catch
            {
                return NotFound();
            }

            var fileInfo = new FileInfo(filePath);
            var lastModified = fileInfo.LastWriteTimeUtc;
            //var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);
            var fileSize = fileInfo.Length;

            var tempDir = GetAppDirectory("Temp");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var md5 = "";
            var previewPath = "";
            if (_usePreviewCache)
            {
                var previewDir = GetAppDirectory("Preview");
                var parentDir = Path.Combine($"{worknum}", Path.GetDirectoryName(path) ?? "");
                if (string.IsNullOrWhiteSpace(parentDir))
                    throw new Exception("PreviewPath is not found parentDir.");

                var previewSubDir = Path.GetFullPath(Path.Combine(previewDir, parentDir));
                if (!previewSubDir.StartsWith(previewDir))
                    throw new Exception("PreviewPath is outside of the previewDir.");

                if (!Directory.Exists(previewSubDir))
                    Directory.CreateDirectory(previewSubDir);

                var meta = new
                {
                    fps = fps,
                    split = split,
                    scale = scale,
                    fileSize = fileSize,
                    lastWriteTime = stringSegment.ToString(),
                    filePath = filePath,
                    workDir = workDir,
                    worknum = worknum
                };
                md5 = JsonConvert.SerializeObject(meta).ToMD5();
                previewPath = Path.Combine(previewSubDir, $"{md5}_preview");
                if (System.IO.File.Exists(previewPath))
                {
                    var fs = new FileStream(previewPath, FileMode.Open, FileAccess.Read);
                    //return File(fs, "image/webp");
                    return File(fs, "image/webp",
                        new DateTimeOffset(lastModified), entityTag);
                }
            }

            var duration = GetVideoDuration(filePath);

            var span = duration < 30 ? duration / (double)split : (duration - 20) / (double)split;

            var times = new List<long>();
            var start = duration < 30 ? 0d : 10d;
            for (var i = 0; i < split; i++)
            {
                times.Add((long)start);
                start = start + span;
                if ((long)start >= duration)
                    break;
            }

            var guid = _usePreviewCache ? md5 : Guid.NewGuid().ToString();

            var tempPath = Path.Combine(tempDir, $"{guid}");
            var outputPath = Path.Combine(tempDir, $"{guid}_output");

            //var success = true;
            //try
            //{
            //    var tasks = new List<Task>();
            //    var processList = new List<Process>();
            //    for (var i = 0; i < times.Count; i++)
            //    {
            //        var ss = times[i];
            //        var arguments = $@"-ss {ss} -t 1 -i ""{filePath}"" -vf ""fps={fps},scale={scale}:force_original_aspect_ratio=decrease"" -vframes {fps} -c:v png -f image2 -start_number {i * fps} ""{tempPath}%04d"" -loglevel error";
            //        var info = new ProcessStartInfo("ffmpeg", arguments);
            //        info.UseShellExecute = false;
            //        var process = Process.Start(info);
            //        var task = process?.WaitForExitAsync();
            //        if (process != null)
            //            processList.Add(process);
            //        if (task != null)
            //            tasks.Add(task);
            //    }
            //    Task.WaitAll(tasks.ToArray());
            //    foreach (var process in processList)
            //    {
            //        if (process.ExitCode != 0)
            //            success = false;
            //        process.Dispose();
            //    }

            //    // png to webp
            //    if (success)
            //    {
            //        var arguments = $@"-r {fps} -f image2 -i ""{tempPath}%04d"" -lossless 0 -qscale 75 -compression_level 0 -loop 0 -f webp ""{outputPath}"" -loglevel error";
            //        var info = new ProcessStartInfo("ffmpeg", arguments);
            //        info.UseShellExecute = false;
            //        var process = Process.Start(info) ??
            //            throw new Exception("Process is null.");
            //        await process.WaitForExitAsync();
            //        if (process.ExitCode != 0)
            //            success = false;
            //        process.Dispose();
            //    }

            //    if (!success)
            //        throw new Exception($"FFmpeg failed.");

            //    using (var fs = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
            //    {
            //        Response.ContentType = "image/webp";
            //        await WriteToBody(fs);
            //        return new EmptyResult();
            //    }
            //}
            //finally
            //{
            //    for (var i = 0; i < fps * times.Count + 0; i++)
            //    {
            //        var image = $"{tempPath}{i.ToString("d4")}";
            //        if (System.IO.File.Exists(image))
            //            System.IO.File.Delete(image);
            //    }
            //    if (success && _usePreviewCache)
            //    {
            //        if (System.IO.File.Exists(outputPath))
            //            System.IO.File.Move(outputPath, previewPath);
            //    }
            //    else
            //    {
            //        if (System.IO.File.Exists(outputPath))
            //            System.IO.File.Delete(outputPath);
            //    }
            //}

            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                {
                    var tasks = new List<Task<bool>>();

                    for (var _i = 0; _i < times.Count; _i++)
                    {
                        var i = _i;

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                var ss = times[i];
                                var arguments = $@"-ss {ss} -t 1 -i ""{filePath}"" -vf ""fps={fps},scale={scale}:force_original_aspect_ratio=decrease"" -vframes {fps} -c:v png -f image2 -start_number {i * fps} ""{tempPath}%04d"" -loglevel error";
                                var info = new ProcessStartInfo("ffmpeg", arguments);
                                info.UseShellExecute = false;
                                using (var process = Process.Start(info))
                                {
                                    if (process == null)
                                        throw new Exception("Failed to start ffmpeg.");
                                    try
                                    {
                                        await process.WaitForExitAsync(cts.Token);
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        process.Kill();
                                        throw new Exception("FFmpeg process timeout.");
                                    }
                                    if (process.ExitCode != 0)
                                        throw new Exception($"FFmpeg failed.");
                                }
                                return true;
                            }
                            catch
                            {
                                if (!cts.IsCancellationRequested)
                                {
                                    cts.Cancel();
                                }
                                return false;
                            }
                        }));
                    }
                    var results = await Task.WhenAll(tasks);
                    if (!results.All(r => r))
                        throw new Exception($"Failed to generate the preview with ffmpeg.");

                    // png to webp
                    {
                        var arguments = $@"-r {fps} -f image2 -i ""{tempPath}%04d"" -lossless 0 -qscale 75 -compression_level 0 -loop 0 -f webp ""{outputPath}"" -loglevel error";
                        var info = new ProcessStartInfo("ffmpeg", arguments);
                        info.UseShellExecute = false;
                        using (var process = Process.Start(info))
                        {
                            if (process == null)
                                throw new Exception("Failed to start ffmpeg.");
                            try
                            {
                                await process.WaitForExitAsync(cts.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                process.Kill();
                                throw new Exception("FFmpeg process timeout.");
                            }
                            if (process.ExitCode != 0)
                                throw new Exception($"FFmpeg failed.");
                        }
                    }
                }

                if (_usePreviewCache)
                {
                    if (System.IO.File.Exists(outputPath))
                        System.IO.File.Move(outputPath, previewPath);

                    var fs = new FileStream(previewPath, FileMode.Open, FileAccess.Read);
                    return File(fs, "image/webp",
                        new DateTimeOffset(lastModified), entityTag);
                }
                else
                {
                    var stream = new MemoryStream();
                    using (var fs = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                    {
                        await fs.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/webp",
                        new DateTimeOffset(lastModified), entityTag);
                }
            }
            catch
            {
                return StatusCode(500, "Preview processing error.");
            }
            finally
            {
                try
                {
                    for (var i = 0; i < fps * times.Count; i++)
                    {
                        var image = $"{tempPath}{i.ToString("d4")}";
                        if (System.IO.File.Exists(image))
                            System.IO.File.Delete(image);
                    }
                    if (System.IO.File.Exists(outputPath))
                        System.IO.File.Delete(outputPath);
                }
                catch { }
            }
        }

        [Obsolete]
        [HttpGet("video/preview/mp4/{worknum}/{*path}")]
        public IActionResult VideoPreviewMP4(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
            }
            catch
            {
                return NotFound();
            }

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var duration = GetVideoDuration(filePath);

            var split = 9;
            var span = duration < 30 ? duration / (double)split : (duration - 20) / (double)split;

            var times = new List<long>();
            var start = duration < 30 ? 0d : 10d;
            for (var i = 0; i < split; i++)
            {
                times.Add((long)start);
                start = start + span;
                if ((long)start >= duration)
                    break;
            }

            var tempDir = GetAppDirectory("Temp");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var guid = Guid.NewGuid().ToString();

            var stream = new MemoryStream();
            var tempPaths = new List<string>();
            var listPath = Path.Combine(tempDir, $"{guid}_concat.txt");
            var concatPath = Path.Combine(tempDir, $"{guid}_concat");

            try
            {
                for (var i = 0; i < times.Count; i++)
                {
                    var ss = times[i];
                    var tempPath = Path.Combine(tempDir, $"{guid}_{i}");
                    tempPaths.Add(tempPath);
                    var arguments = $@"-ss {ss} -t 1 -i ""{filePath}"" -vf scale=320:-2 -c:v libx264 -r 24 -an -f mp4 ""{tempPath}"" -loglevel error";
                    var info = new ProcessStartInfo("ffmpeg", arguments);
                    info.UseShellExecute = false;
                    var process = Process.Start(info);
                    process?.WaitForExit();
                    process?.Dispose();
                }

                System.IO.File.WriteAllText(listPath,
                    string.Join("\n", tempPaths.Select(it => $@"file '{it}'")));
                {
                    var arguments = $@"-safe 0 -f concat -i ""{listPath}"" -f mp4 ""{concatPath}"" -loglevel error";
                    var info = new ProcessStartInfo("ffmpeg", arguments);
                    info.UseShellExecute = false;
                    var process = Process.Start(info);
                    process?.WaitForExit();
                    process?.Dispose();
                }

                using (var fs = new FileStream(
                    concatPath, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(stream);
                }
            }
            finally
            {
                foreach (var tempPath in tempPaths)
                {
                    if (System.IO.File.Exists(tempPath))
                        System.IO.File.Delete(tempPath);
                }
                if (System.IO.File.Exists(listPath))
                    System.IO.File.Delete(listPath);
                if (System.IO.File.Exists(concatPath))
                    System.IO.File.Delete(concatPath);
            }

            stream.Position = 0;
            return File(stream, "video/mp4");
        }

        private long GetVideoDuration(string filePath)
        {
            var arguments = $@"-i ""{filePath}""";
            var info = new ProcessStartInfo("ffmpeg", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            using (var process = Process.Start(info))
            {
                if (process == null)
                    throw new Exception("Process is null.");
                var videoInfo = process.StandardError.ReadToEnd();
                process.WaitForExit();
                var durationText = Regex.Match(videoInfo, @"Duration: (\d{2}):(\d{2}):(\d{2})");
                var duration =
                    int.Parse(durationText.Groups[1].Value) * 60 * 60 +
                    int.Parse(durationText.Groups[2].Value) * 60 +
                    int.Parse(durationText.Groups[3].Value);
                return duration;
            }
        }

        [HttpGet("image/{worknum}/{*path}")]
        public async Task<IActionResult> _Image(int worknum, string path)
        {
            var scaleW = 320;
            var scaleH = 240;

            var workDir = "";
            var filePath = "";
            try
            {
                workDir = _workDirs[worknum - 1].Path;
                filePath = Path.GetFullPath(Path.Combine(workDir, path));
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
                if (!filePath.StartsWith(workDir))
                    throw new Exception("Path is outside of the workDir.");
            }
            catch
            {
                return NotFound();
            }

            var fileInfo = new FileInfo(filePath);
            var lastModified = fileInfo.LastWriteTimeUtc;
            //var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);
            var fileSize = fileInfo.Length;

            var md5 = "";
            var thumbnailPath = "";
            if (_useThumbnailCache)
            {
                var thumbnailDir = GetAppDirectory("Thumbnail");
                var parentDir = Path.Combine($"{worknum}", Path.GetDirectoryName(path) ?? "");
                if (string.IsNullOrWhiteSpace(parentDir))
                    throw new Exception("ThumbnailPath is not found parentDir.");

                var thumbnailSubDir = Path.GetFullPath(Path.Combine(thumbnailDir, parentDir));
                if (!thumbnailSubDir.StartsWith(thumbnailDir))
                    throw new Exception("ThumbnailPath is outside of the thumbnailDir.");

                if (!Directory.Exists(thumbnailSubDir))
                    Directory.CreateDirectory(thumbnailSubDir);

                var meta = new
                {
                    scale = $"{scaleW}:{scaleH}",
                    fileSize = fileSize,
                    lastWriteTime = stringSegment.ToString(),
                    filePath = filePath,
                    workDir = workDir,
                    worknum = worknum
                };
                md5 = JsonConvert.SerializeObject(meta).ToMD5();
                thumbnailPath = Path.Combine(thumbnailSubDir, $"{md5}_thumbnail");
                if (System.IO.File.Exists(thumbnailPath))
                {
                    var fs = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);
                    //return File(fs, "image/jpeg");
                    return File(fs, "image/jpeg",
                        new DateTimeOffset(lastModified), entityTag);
                }
            }

            var stream = new MemoryStream();
            CreateImageThumbnail(filePath, scaleW, scaleH, stream);
            stream.Position = 0;

            if (!_useThumbnailCache)
                //return File(stream, "image/jpeg");
                return File(stream, "image/jpeg",
                    new DateTimeOffset(lastModified), entityTag);

            var tempDir = GetAppDirectory("Temp");
            var tempPath = Path.Combine(tempDir, $"{md5}_thumbnail");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            //var success = false;
            //try
            //{
            //    using (var fs = System.IO.File.Create(tempPath))
            //    {
            //        stream.CopyTo(fs);
            //    }

            //    success = true;

            //    using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
            //    {
            //        Response.ContentType = "image/jpeg";
            //        await WriteToBody(fs);
            //        return new EmptyResult();
            //    }
            //}
            //finally
            //{
            //    if (success)
            //    {
            //        if (System.IO.File.Exists(tempPath))
            //            System.IO.File.Move(tempPath, thumbnailPath);
            //    }
            //    else
            //    {
            //        if (System.IO.File.Exists(tempPath))
            //            System.IO.File.Delete(tempPath);
            //    }
            //}

            try
            {
                using (var _fs = System.IO.File.Create(tempPath))
                {
                    stream.CopyTo(_fs);
                }
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Move(tempPath, thumbnailPath);

                var fs = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);
                return File(fs, "image/jpeg",
                    new DateTimeOffset(lastModified), entityTag);
            }
            catch
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
                return StatusCode(500, "Temp thumbnail processing error.");
            }
        }

        private void CreateImageThumbnail(string filePath, int fixWidth, int fixHeight, Stream output)
        {
            using (var image = Image.Load(filePath))
            {
                if (image.Width <= fixWidth && image.Height <= fixHeight)
                {
                    using (var fs = new FileStream(
                        filePath, FileMode.Open, FileAccess.Read))
                    {
                        fs.CopyTo(output);
                    }
                    return;
                }

                var widthScale = (double)fixWidth / image.Width;
                var heightScale = (double)fixHeight / image.Height;
                var scale = widthScale < heightScale ? widthScale : heightScale;
                var width = (int)(image.Width * scale);
                var height = (int)(image.Height * scale);

                image.Mutate(it => it.Resize(width, height));
                image.SaveAsJpeg(output, new JpegEncoder
                {
                    Quality = 90
                });
            }
        }
    }
}