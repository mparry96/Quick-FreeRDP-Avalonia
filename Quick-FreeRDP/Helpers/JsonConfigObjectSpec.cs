using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.Helpers;

public class JsonConfigObjectSpec
{
    public ObservableCollection<string> AvailableResolutions { get; set; } = new();

    public ObservableCollection<RdpItem> RdpItems { get; set; } = new();
    
}