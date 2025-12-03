namespace Filer.Models
{
    public enum FileType
    {
        WorkDir,
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
        public int WorkNum { get; set; } = 0;
        public string WorkDir { get; set; } = "";
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
        public string DirPath { get; set; } = "";
        public string DirName { get; set; } = "";
        public string ParentDirPath { get; set; } = "";
        public string ParentDirName { get; set; } = "";
        public string MimeType { get; set; } = "";
        public string FileSize { get; set; } = "";
        public long FileLength { get; set; } = 0;
        public int ItemCount { get; set; } = 0;
        public DateTime LastWriteTimeUtc { get; set; }
        public string LastWriteTimeUtcText { get; set; } = "";
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public bool HasHistory { get; set; } = false;
        public int HistoryCount { get; set; } = 0;
        public int Index { get; set; } = 0;
        public bool IsPathError { get; set; } = false;
        public string NameTW { get; set; } = "";
    }
}