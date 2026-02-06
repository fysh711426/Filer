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

        public static (string path, string pathName, string parentPath, string parentName, string grandParentPath, string grandParentName)
            GetPathInfo(string path)
        {
            path = path.Trim('/').Trim('\\').Replace(@"\", "/") ?? "";
            var parentPath = string.IsNullOrWhiteSpace(path) ? "" :
                Path.GetDirectoryName(path)?.Replace(@"\", "/") ?? "";
            var grandParentPath = string.IsNullOrWhiteSpace(parentPath) ? "" :
                Path.GetDirectoryName(parentPath)?.Replace(@"\", "/") ?? "";
            return (
                path,
                Path.GetFileName(path),
                parentPath,
                Path.GetFileName(parentPath),
                grandParentPath,
                Path.GetFileName(grandParentPath)
            );
        }

        public static string GetDirNameTitle(string dirName, string workDir)
        {
            if (!string.IsNullOrWhiteSpace(dirName))
                return dirName;
            if (!string.IsNullOrWhiteSpace(workDir))
                return workDir;
            return "";
        }
    }
}