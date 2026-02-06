namespace Filer.Extensions
{
    public static class Helper
    {
        public static string FormatFileSize(double fileSize)
        {
            if (fileSize < 0)
            {
                return "Error";
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                var size = fileSize / (1024 * 1024 * 1024);
                return string.Format("{0:########0.00} GB",
                    Math.Floor(size * 100) / 100);
            }
            else if (fileSize >= 1024 * 1024)
            {
                var size = fileSize / (1024 * 1024);
                return string.Format("{0:####0.00} MB",
                    Math.Floor(size * 100) / 100);
            }
            else if (fileSize >= 1024)
            {
                var size = fileSize / 1024;
                return string.Format("{0:####0.00} KB",
                    Math.Floor(size * 100) / 100);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }
        }
    }
}