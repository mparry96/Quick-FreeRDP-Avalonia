using System;
using System.Collections.ObjectModel;
using System.Linq;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.Helpers;

public static class SortHelper
{
    public static void SortByName(ObservableCollection<RdpItem> collection)
    {
        var sorted = collection
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        collection.Clear();

        foreach (var item in sorted)
        {
            collection.Add(item);
        }
    }
}