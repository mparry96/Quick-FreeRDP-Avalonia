using CommunityToolkit.Mvvm.ComponentModel;

namespace Quick_FreeRDP.Models;

public partial class RdpItem : ObservableObject
{
    // [ObservableProperty]
    // private string name = string.Empty;
    
    private string name = string.Empty;

    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }
    
    public string IpAddress { get; set; }  = string.Empty;
    
    public string UserName { get; set; }  = string.Empty;
    
    public string Domain { get; set; }  = string.Empty;
    
    [ObservableProperty]
    public partial int ResolutionHeight { get; set; }

    [ObservableProperty]
    public partial int ResolutionWidth { get; set; }
    
    public bool FullScreenBool { get; set; }
    
    public bool FloatBarBool { get; set; }
    
    
}