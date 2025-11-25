namespace Filer.Models
{
    public enum FileType
    {
        Folder,
        Video,
        Audio,
        Image,
        Text,
        Other
    }

    public class FileModel
    {
        public FileType FileType { get; set; } = FileType.Other;
        public string Path { get; set; } = "";
        public string MimeType { get; set; } = "";
        public string Name { get; set; } = "";
        public string FileSize { get; set; } = "";
        public long FileLength { get; set; } = 0;
        public int ItemCount { get; set; } = 0;
        public DateTime LastWriteTimeUtc { get; set; }
        public string LastWriteTimeUtcText { get; set; } = "";
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public bool HasHistory { get; set; } = false;
        public int HistoryCount { get; set; } = 0;
    }
}