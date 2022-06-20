namespace FileBrowser.Models
{
    public enum FileType
    {
        Other = 0,
        Folder,
        Video,
        Audio,
        Image,
        Text
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
