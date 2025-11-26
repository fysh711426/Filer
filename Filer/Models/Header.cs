namespace Filer.Models
{
    public enum HeaderMode
    {
        Theme,
        TextTheme
    }

    public class Header
    {
        public HeaderMode Mode { get; set; } = HeaderMode.Theme;

        public string Page { get; set; } = "";

        public Header(HeaderMode mode, string page = "")
        {
            Mode = mode;
            Page = page;
        }
    }
}