namespace Filer.Extensions
{
    public static class MimeTypeHelper
    {
        public static readonly Dictionary<string, bool> ImageMimeType = new()
        {
            ["image/jpeg"] = true,
            ["image/png"] = true,
            ["image/gif"] = true,
            ["image/webp"] = true,
            ["image/bmp"] = true
        };

        public static readonly Dictionary<string, bool> VideoMimeType = new()
        {
            ["video/mp4"] = true,
            ["video/webm"] = true,
            ["video/quicktime"] = true,           // mov
            ["video/x-msvideo"] = true,           // avi
            ["video/x-ms-wmv"] = true,            // wmv
            ["video/vnd.dlna.mpeg-tts"] = true,   // mp2t
            ["video/x-matroska"] = true,          // mkv
            ["video/x-flv"] = true,               // flv
            ["audio/x-mpegurl"] = true            // m3u8
        };

        public static readonly Dictionary<string, bool> AudioMimeType = new()
        {
            ["audio/mpeg"] = true
        };

        public static readonly Dictionary<string, bool> TextMimeType = new()
        {
            ["text/plain"] = true,
            ["text/vtt"] = true
        };
    }
}