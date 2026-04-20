using CommunityToolkit.Mvvm.ComponentModel;

namespace Quick_FreeRDP.Models;

public class RdpItem //: ObservableObject
{
    public  string Name { get; set; } = string.Empty;
    
    public string IpAddress { get; set; }  = string.Empty;
    
    public int ResolutionHeight { get; set; }

    public int ResolutionWidth { get; set; }

    public bool FullScreenBool { get; set; }
    
    public bool FloatBarBool { get; set; }
    
    
}