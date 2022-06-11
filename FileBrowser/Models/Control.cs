namespace FileBrowser.Models
{
    public class Control
    {
        public bool ShowHome { get; set; }
        public Control(bool showHome = true)
        {
            ShowHome = showHome;
        }
    }
}
