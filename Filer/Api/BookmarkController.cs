using Filer.Api.Shared;
using Filer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using static Filer.Extensions.PathHelper;

namespace Filer.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : BaseController
    {
        public BookmarkController(IConfiguration configuration)
            : base(configuration)
        {
        }

        //[HttpGet("list")]
        //public ActionResult<List<string>> GetList()
        //{
        //    var bookmarkDir = GetAppDirectory("Bookmark");
        //    if (!Directory.Exists(bookmarkDir))
        //        return Ok(new List<string>());

        //    var files = Directory.EnumerateFiles(bookmarkDir)
        //        .Select(it => Path.GetFileName(it))
        //        .Where(it => it.StartsWith("bookmarks_"))
        //        .OrderByDescending(it => it)
        //        .ToList();

        //    return Ok(files);
        //}

        [HttpGet("sync")]
        //[HttpGet("sync/{fileName}")]
        public async Task<IActionResult> Sync()
        {
            var bookmarkDir = GetAppDirectory("Bookmark");
            var files = new List<string>();
            if (Directory.Exists(bookmarkDir))
            {
                files = Directory.EnumerateFiles(bookmarkDir)
                    .Select(it => Path.GetFileName(it))
                    .Where(it => it.StartsWith("bookmarks_"))
                    .OrderByDescending(it => it)
                    .ToList();
            }
            if (files.Count == 0)
                return Content("null", "application/json");
            var filePath = Path.Combine(bookmarkDir, files[0]);
            if (!System.IO.File.Exists(filePath))
                throw new Exception("File not found.");
            if (!filePath.StartsWith(bookmarkDir))
                throw new Exception("File is outside of the bookmarkDir.");
            var content = await System.IO.File.ReadAllTextAsync(filePath);
            return Content(content, "application/json");
        }

        [HttpPost("backup")]
        public async Task<IActionResult> Backup([FromBody] Bookmarks bookmarks)
        {
            var bookmarkDir = GetAppDirectory("Bookmark");
            if (!Directory.Exists(bookmarkDir))
                Directory.CreateDirectory(bookmarkDir);

            var time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = $"bookmarks_{time}.json";
            var filePath = Path.Combine(bookmarkDir, fileName);
            if (System.IO.File.Exists(filePath))
                throw new Exception("File already exists.");
            if (!filePath.StartsWith(bookmarkDir))
                throw new Exception("File is outside of the bookmarkDir.");

            var json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { Modifier } }
            });
            await System.IO.File.WriteAllTextAsync(filePath, json);

            return Ok();
        }

        protected static void Modifier(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Type == typeof(Bookmarks))
            {
                foreach (var property in typeInfo.Properties)
                {
                    if (property.Name.Equals("FileName", StringComparison.OrdinalIgnoreCase))
                    {
                        property.ShouldSerialize = (obj, val) => false;
                    }
                }
            }
        }
    }
}