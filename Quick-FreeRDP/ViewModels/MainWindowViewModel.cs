using  System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Quick_FreeRDP.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    public string RdpName { get; set; } = string.Empty;
    
    public string IpAddress { get; set; } = string.Empty;

    public int ResolutionWidth { get; set; }
    
    public int ResolutionHeight { get; set; }

    public bool FullScreenBool { get; set; } = false;

    public bool FloatBarBool { get; set; } = false;
    
    public List<string> RdpItems { get; set; } = new()
    {
        "Option 1",
        "Option 2",
        "Option 3"
    };
    
    
    
}