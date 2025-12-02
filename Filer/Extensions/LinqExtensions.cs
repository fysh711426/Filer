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

        public static IEnumerable<T> DebugEnumerable<T>(this IEnumerable<T> source, string title)
        {
            var count = 0;
            foreach (var item in source)
            {
                Console.WriteLine($"{title}: {count++}");
                yield return item;
            }
        }
    }
}