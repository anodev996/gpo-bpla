using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class E_Array
{
    public static T GetRandomItem<T>(this T[] array)
    {
        if (array.Length == 0)
            return default;
        return array[Random.Range(0, array.Length)];
    }
    public static T GetRandomItem<T>(this IList<T> array)
    {
        return array[Random.Range(0, array.Count)];
    }
    public static int GetRandomIndex<T>(this T[] array)
    {
        return Random.Range(0, array.Length);
    }
    public static int GetRandomIndex<T, Y>(this Dictionary<T, Y>.ValueCollection array)
    {
        return Random.Range(0, array.Count);
    }
    public static T GetKey<T, Y>(this Dictionary<T, Y> _dict, Y value)
        where T : class
        where Y : class
    {
        var dict = _dict.ToArray();
        for (int i = 0; i < dict.Length; i++)
        {
            if (dict[i].Value == value)
            {
                return dict[i].Key;
            }
        }
        return default;
    }
    public static int GetIndexArray(this System.Array array, object item)
    {
        return array.ToListObject<object>().GetIndex(item);
    }
    public static int GetIndexArray<T>(this System.Array array, T item) where T : System.IEquatable<T>
    {
        return array.ToListObject<T>().GetIndexEquals(item);
    }
    public static List<T> ToListObject<T>(this System.Array array)
    {
        var list = new List<T>();
        for (int i = 0; i < array.Length; i++)
        {
            list.Add((T)(array.GetValue(i)));
        }
        return list;
    }
    public static int GetIndexEquals<T>(this IList<T> array, T item) where T : System.IEquatable<T>
    {
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }
    public static int GetIndex<T>(this IList<T> array, T item) where T : class
    {
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] == item)
            {
                return i;
            }
        }
        return -1;
    }
    public static string WordByIndex(this string text, int index)
    {
        int start = index;
        int end = index;

        while (start >= 1 && char.IsLetterOrDigit(text[start - 1]))
        {
            start--;
        }
        while (end < text.Length && char.IsLetterOrDigit(text[end]))
        {
            end++;
        }
        return text.Substring(start, end - start);
    }
    public static T[] Add<T>(this T[] array, T item)
    {
        if (array == null)
            return new T[1] { item };
        var list = new List<T>(array);
        list.Add(item);
        return list.ToArray();
    }
    public static T[] Add<T>(this T[] array, T[] item)
    {
        if (array == null)
            return item;
        var list = new List<T>(array);
        list.AddRange(item);
        return list.ToArray();
    }
    public static T[] Remove<T>(this T[] array, T item)
    {
        if (array == null)
            return null;
        var list = new List<T>(array);
        list.Remove(item);
        return list.ToArray();
    }
    public static bool Equally<T>(this T[] array, T[] item) where T : class
    {
        if (array.Length != item.Length)
        {
            return false;
        }
        for (int i = 0; i < array.Length; i++)
            if (array[i] != item[i])
                return false;
        return true;
    }
    public static string ToStringList<T>(this IList<T> ts)
    {
        if (ts == null)
            return "null";
        string item = $"Count: {ts.Count} ";
        item += "{ ";
        for (ushort i = 0; i < ts.Count; i++)
        {
            item += $" {ts[i]}\n";
        }
        item += " }";
        return item;
    }
    public static string ToStringList<T>(this IEnumerable<T> ts)
    {
        return ToStringList(ts.ToArray());
    }
    public static string ToStringDictionary<T, K>(this Dictionary<T, K> ts)
    {
        if (ts == null)
            return "null";
        string item = $"Count: {ts.Count} ";
        item += "{ ";
        for (ushort i = 0; i < ts.Count; i++)
        {
            item += $" {ts.Keys.ElementAt(i)}:{ts.Values.ElementAt(i)}\n";
        }
        item += " }";
        return item;
    }
    public static string ToStringList<T>(this T[,] ts)
    {
        if (ts == null)
            return "null";
        string item = $"Count: {ts.Length} ";
        item += "{\n";
        for (int x = 0; x < ts.GetLength(0); x++)
        {
            for (int y = 0; y < ts.GetLength(1); y++)
            {
                item += $" {ts[x, y]}";
            }
            item += "\n";
        }
        item += " }";
        return item;
    }
    public static string ToStringRayList(this IList<RaycastHit2D> ts)
    {
        if (ts == null)
            return "null";
        string item = $"Count: {ts.Count} ";
        item += "{ ";
        for (ushort i = 0; i < ts.Count; i++)
        {
            item += ts[i].ToStringRay();
        }
        item += " }";
        return item;
    }
    public static string ToStringRay(this RaycastHit2D hit)
    {
        return $"(point: {hit.point} \n distance: {hit.distance} \n collider: {hit.collider}) \n";
    }
    public static T[] Rotate<T>(this T[] item)
    {
        T[] result = new T[item.Length];
        for (int i = 1; i < item.Length + 1; i++)
            result[i - 1] = item[item.Length - i];
        return result;
    }
    public static T GetItemFilter<T>(this T[] item, System.Func<T, bool> filter)
    {
        for (int i = 0; i < item.Length; i++)
            if (filter.Invoke(item[i]))
                return item[i];
        return default;
    }
    public static T GetItemFilter<T>(this T[] item, System.Func<T, T, bool> filter)
    {
        T obj = item[0];
        for (int i = 0; i < item.Length; i++)
            if (filter.Invoke(item[i], obj))
                obj = item[i];
        return obj;
    }
    public static Y[] Filter<T, Y>(this IList<T> item, System.Func<T, Y> filter)
    {
        Y[] array = new Y[item.Count];
        for (int i = 0; i < item.Count; i++)
            array[i] = filter.Invoke(item[i]);
        return array;
    }
    public static T[] Combine<T>(this T[][] ts)
    {
        var list = new List<T>();
        for (int i = 0; i < ts.Length; i++)
            for (int x = 0; x < ts[i].Length; x++)
                list.Add(ts[i][x]);
        return list.ToArray();
    }
    public static bool Contains<T>(this IList<T> item, T value) where T : class
    {
        for (int i = 0; i < item.Count; i++)
            if (item[i] == value)
                return true;
        return false;
    }
    public static bool Contains(this IList<Vector2Int> item, Vector2Int value)
    {
        for (int i = 0; i < item.Count; i++)
            if (item[i] == value)
                return true;
        return false;
    }
    public static bool Contains<T>(this IList<T> item, System.Func<T, bool> filter)
    {
        if (item == null)
            return false;
        if (item.Count == 0)
            return false;
        for (int i = 0; i < item.Count; i++)
            if (filter.Invoke(item[i]))
                return true;
        return false;
    }
    public static Y[] FilterNoNull<T, Y>(this IList<T> item, System.Func<T, Y> filter)
    {
        List<Y> items = new List<Y>();
        for (int i = 0; i < item.Count; i++)
            items.Add(filter.Invoke(item[i]));
        items.RemoveAll((item) => item == null);
        return items.ToArray();
    }
    public static float EqualsDegree<T>(this IList<T> array, IList<T> list, System.Func<T, T, bool> equals)
    {
        if (array.Count != list.Count)
            return 0;
        int equalsItemCount = 0;
        for (int i = 0; i < array.Count; i++)
        {
            if (equals.Invoke(array[i], list[i]))
            {
                equalsItemCount++;
            }
        }
        return equalsItemCount / array.Count;
    }
    public static void RemoveRange<T>(this IList<T> list, IList<T> range)
    {
        for (int i = 0; i < range.Count; i++)
            list.Remove(range[i]);
    }
    public static T GetFirstNoNull<T>(this T[] array) where T : class
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
                return array[i];
        }
        return null;
    }
    public static T[] Fill<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] = value;
        return array;
    }
}