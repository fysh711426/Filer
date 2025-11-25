namespace Filer.Models
{
    public class WorkDir
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public int Index { get; set; } = 0;
        public bool IsPathError { get; set; } = false;
        public int ItemCount { get; set; } = 0;
        public bool HasHistory { get; set; } = false;
        public int HistoryCount { get; set; } = 0;
    }
}