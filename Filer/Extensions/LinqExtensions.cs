using Filer.Models;

namespace Filer.Extensions
{
    public static class LinqExtensions
    {
        public static IOrderedEnumerable<FileModel> ThenByNatural(this IOrderedEnumerable<FileModel> orderDatas,
            Func<FileModel, string> keySelector, bool useWindowsNaturalSort) =>
                !useWindowsNaturalSort ?
                    orderDatas.ThenBy(keySelector) :
                    orderDatas.ThenBy(keySelector, new WindowsNaturalSort());

        public static IOrderedEnumerable<FileModel> ThenByNaturalDescending(this IOrderedEnumerable<FileModel> orderDatas,
            Func<FileModel, string> keySelector, bool useWindowsNaturalSort) =>
                !useWindowsNaturalSort ?
                    orderDatas.ThenByDescending(keySelector) :
                    orderDatas.ThenByDescending(keySelector, new WindowsNaturalSort());
    }
}