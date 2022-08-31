namespace FileBrowser.Models
{
    public enum HeaderMode
    {
        Theme,
        TextTheme
    }

    public class Header
    {
        public HeaderMode Mode { get; set; }

        public Header(HeaderMode mode)
        {
            Mode = mode;
        }
    }
}