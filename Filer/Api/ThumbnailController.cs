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
        public IActionResult _Video(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var arguments = $@"-ss 00:00:01.00 -i ""{filePath}"" -vf ""scale=480:360:force_original_aspect_ratio=decrease"" -vframes 1 -c:v png -f image2 pipe: -loglevel error";
            var info = new ProcessStartInfo("ffmpeg", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            var process = Process.Start(info) ??
                throw new Exception("Process is null.");
            return File(process.StandardOutput.BaseStream, "image/png");
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
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);
            var fileSize = new FileInfo(filePath).Length;

            var tempDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var md5 = "";
            var previewPath = "";
            if (_usePreviewCache)
            {
                var previewDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Preview");
                if (!Directory.Exists(previewDir))
                    Directory.CreateDirectory(previewDir);

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
                previewPath = Path.Combine(previewDir, $"{md5}_preview");
                if (System.IO.File.Exists(previewPath))
                {
                    var fs = new FileStream(previewPath, FileMode.Open, FileAccess.Read);
                    return File(fs, "image/webp");
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

            try
            {
                var tasks = new List<Task>();
                var processList = new List<Process>();
                for (var i = 0; i < times.Count; i++)
                {
                    var ss = times[i];
                    var arguments = $@"-ss {ss} -t 1 -i ""{filePath}"" -vf ""fps={fps},scale={scale}:force_original_aspect_ratio=decrease"" -c:v png -f image2 -start_number {i * fps} ""{tempPath}%04d"" -loglevel error";
                    var info = new ProcessStartInfo("ffmpeg", arguments);
                    info.UseShellExecute = false;
                    var process = Process.Start(info);
                    var task = process?.WaitForExitAsync();
                    if (process != null)
                        processList.Add(process);
                    if (task != null)
                        tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                foreach (var process in processList)
                {
                    process.Dispose();
                }

                // png to webp
                {
                    var arguments = $@"-r {fps} -f image2 -i ""{tempPath}%04d"" -lossless 0 -qscale 75 -compression_level 0 -loop 0 -f webp ""{outputPath}"" -loglevel error";
                    var info = new ProcessStartInfo("ffmpeg", arguments);
                    info.UseShellExecute = false;
                    var process = Process.Start(info) ??
                        throw new Exception("Process is null.");
                    await process.WaitForExitAsync();
                    process.Dispose();
                }

                using (var fs = new FileStream(
                    outputPath, FileMode.Open, FileAccess.Read))
                {
                    Response.ContentType = "image/webp";
                    await WriteToBody(fs);
                    return new EmptyResult();
                }
            }
            finally
            {
                for (var i = 0; i < fps * times.Count + 0; i++)
                {
                    var image = $"{tempPath}{i.ToString("d4")}";
                    if (System.IO.File.Exists(image))
                        System.IO.File.Delete(image);
                }
                if (_usePreviewCache)
                {
                    if (System.IO.File.Exists(outputPath))
                        System.IO.File.Move(outputPath, previewPath);
                }
                else
                {
                    if (System.IO.File.Exists(outputPath))
                        System.IO.File.Delete(outputPath);
                }
            }
        }

        [HttpGet("video/preview/mp4/{worknum}/{*path}")]
        public IActionResult VideoPreviewMP4(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
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

            var tempDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Temp");
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
            var process = Process.Start(info) ??
                throw new Exception("Process is null.");
            var videoInfo = process.StandardError.ReadToEnd();
            var durationText = Regex.Match(videoInfo, @"Duration: (\d{2}):(\d{2}):(\d{2})");
            var duration =
                int.Parse(durationText.Groups[1].Value) * 60 * 60 +
                int.Parse(durationText.Groups[2].Value) * 60 +
                int.Parse(durationText.Groups[3].Value);
            process.Dispose();
            return duration;
        }

        [HttpGet("image/{worknum}/{*path}")]
        public IActionResult _Image(int worknum, string path)
        {
            var filePath = "";
            try
            {
                var workDir = _workDirs[worknum - 1].Path;
                filePath = Path.Combine(workDir, path);
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("Path not found.");
            }
            catch
            {
                return NotFound();
            }

            var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            var stringSegment = (StringSegment)$@"""{lastModified.ToString("yyyyMMddHHmmss")}""";
            var entityTag = new EntityTagHeaderValue(stringSegment);

            var stream = new MemoryStream();
            CreateImageThumbnail(filePath, 320, 240, stream);
            stream.Position = 0;

            return File(stream, "image/jpeg");
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