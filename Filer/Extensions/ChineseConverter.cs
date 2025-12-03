using System.Runtime.InteropServices;

namespace Filer.Extensions
{
    public class ChineseConverter
    {
        private const int zh_TW = 1028;
        private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

        //[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int LCMapString(
            int locale, uint dwMapFlags, [MarshalAs(UnmanagedType.LPWStr)] string lpSrcStr, int cchSrc, IntPtr lpDestStr, int cchDest);

        /// <summary>
        /// 繁體轉簡體
        /// </summary>
        public static string ToSimplified(string text)
        {
            int num = text.Length * 2 + 2;
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            LCMapString(zh_TW, LCMAP_SIMPLIFIED_CHINESE, text, -1, intPtr, num);
            string? result = Marshal.PtrToStringUni(intPtr);
            Marshal.FreeHGlobal(intPtr);
            return result ?? "";
        }

        /// <summary>
        /// 簡體轉繁體
        /// </summary>
        public static string ToTraditional(string text)
        {
            int num = text.Length * 2 + 2;
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            LCMapString(zh_TW, LCMAP_TRADITIONAL_CHINESE, text, -1, intPtr, num);
            string? result = Marshal.PtrToStringUni(intPtr);
            Marshal.FreeHGlobal(intPtr);
            return result ?? "";
        }
    }
}