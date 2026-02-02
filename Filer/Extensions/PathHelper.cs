namespace Filer.Extensions
{
    public static class PathHelper
    {
        public static string GetAppDirectory(string? combineDir = null)
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(combineDir))
                appDir = Path.Combine(appDir, combineDir);
            appDir = appDir.TrimEnd(Path.DirectorySeparatorChar);
            appDir = $@"{appDir}{Path.DirectorySeparatorChar}";
            return appDir;
        }
    }
}