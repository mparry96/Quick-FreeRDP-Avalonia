using CommunityToolkit.Mvvm.ComponentModel;

namespace Quick_FreeRDP.Models;

public partial class RdpItem : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;
    
    public string IpAddress { get; set; }  = string.Empty;
    
    public string UserName { get; set; }  = string.Empty;
    
    public int ResolutionHeight { get; set; }

    public int ResolutionWidth { get; set; }

    public bool FullScreenBool { get; set; }
    
    public bool FloatBarBool { get; set; }
    
    
}