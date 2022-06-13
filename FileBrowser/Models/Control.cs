namespace FileBrowser.Models
{
    public class Control
    {
        public bool ShowHome { get; set; }
        public int WorkNum { get; set; }
        public string FilePath { get; set; }
        public string ParentDirPath { get; set; }
        public Control(
            bool showHome = true,
            int workNum = 1,
            string filePath = "",
            string parentDirPath = "")
        {
            ShowHome = showHome;
            WorkNum = workNum;
            FilePath = filePath;
            ParentDirPath = parentDirPath;
        }
    }
}
