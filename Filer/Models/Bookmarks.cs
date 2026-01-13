namespace Filer.Models
{
    public class Bookmarks
    {
        public string FileName { get; set; } = "";
        public List<BookmarkGroup> Groups { get; set; } = new();
    }

    public class BookmarkGroup
    {
        public string Name { get; set; } = "";
        public List<BookmarkItem> Items { get; set; } = new();
    }

    public class BookmarkItem
    {
        public string Url { get; set; } = "";
        public FileType FileType { get; set; } = FileType.Other;
        public int WorkNum { get; set; } = 0;
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
        public string Link { get; set; } = "";
        public string Thumb { get; set; } = "";
    }
}