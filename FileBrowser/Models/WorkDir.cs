namespace FileBrowser.Models
{
    public class WorkDir
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public int Index { get; set; } = 0;
        public bool IsPathError { get; set; } = false;
    }
}