using System.Runtime.InteropServices;
using System.Security;

namespace Filer.Extensions
{
    [SuppressUnmanagedCodeSecurity]
    public class WindowsNaturalSort : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        public int Compare(string? x, string? y)
        {
            return StrCmpLogicalW(x ?? "", y ?? "");
        }
    }
}