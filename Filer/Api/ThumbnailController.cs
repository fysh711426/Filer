using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
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

            var arguments = $@"-ss 00:00:01.00 -i ""{filePath}"" -vf ""scale=320:240:force_original_aspect_ratio=decrease"" -vframes 1 -f image2 pipe: -loglevel error";
            var info = new ProcessStartInfo("ffmpeg.exe", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            var process = Process.Start(info) ?? 
                throw new Exception("Process is null.");
            return File(process.StandardOutput.BaseStream, "image/jpeg",
                new DateTimeOffset(lastModified), entityTag, true);
        }

        [HttpGet("video/preview/{worknum}/{*path}")]
        public IActionResult VideoPreview(int worknum, string path)
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

            var arguments = $@"-i ""{filePath}"" -filter_complex ""[0:v]select='lt(mod(t,{GetVideoDuration(filePath)}/8),1)',setpts=N/(FRAME_RATE*TB),scale=320:-2"" -an -f webm pipe: -loglevel error";
            var info = new ProcessStartInfo("ffmpeg.exe", arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            var process = Process.Start(info) ??
                throw new Exception("Process is null.");
            return File(process.StandardOutput.BaseStream, "video/webm",
                new DateTimeOffset(lastModified), entityTag, true);
        }

        private long GetVideoDuration(string filePath)
        {
            var arguments = $@"-i ""{filePath}""";
            var info = new ProcessStartInfo("ffmpeg.exe", arguments);
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

            return File(stream, "image/jpeg",
                new DateTimeOffset(lastModified), entityTag, true);
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