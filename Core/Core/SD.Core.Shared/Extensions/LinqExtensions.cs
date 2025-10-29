using System.Collections.ObjectModel;

namespace SD.Core.Shared.Extensions;
public static class LinqExtensions
{
    public static List<List<T>> Split<T>(this List<T> list, int parts)
    {
        int i = 0;
        var splits = from item in list
                     group item by i++ % parts into part
                     select part.AsEnumerable().ToList();
        return splits.ToList();
    }
    public static void SetRange<T>(this ObservableCollection<T>? items, IList<T>? collection)
    {
        if (collection == null)
            return;

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        items.Clear();
        items.AddRange(collection);
    }
    public static void AddRange<T>(this ObservableCollection<T> items, IList<T>? collection)
    {
        if (collection == null)
            return;

        foreach (var i in collection)
            items.Add(i);
    }
}