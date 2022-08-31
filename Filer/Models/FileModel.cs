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
        public string ItemCount { get; set; } = "";
    }
}