using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService;

public static class IEnumerableExtensions
{
    /// <summary>
    ///     Metodo de extension para metodos que implementan IENUMERABLE
    ///     podes
    /// </summary>
    public static IEnumerable<T> Map<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
            yield return item;
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
    {
        foreach (var item in enumerable) await action(item);
    }

    public static ObservableCollection<T> Convert<T>(IEnumerable<T> original)
    {
        return new ObservableCollection<T>(original);
    }

    public static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var dataTable = new DataTable(typeof(T).Name);
        //Get all the properties
        var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in Props)
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name);
        foreach (var item in items)
        {
            var values = new object[Props.Length];
            for (var i = 0; i < Props.Length; i++)
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null);
            dataTable.Rows.Add(values);
        }

        //put a breakpoint here and check datatable
        return dataTable;
    }
}