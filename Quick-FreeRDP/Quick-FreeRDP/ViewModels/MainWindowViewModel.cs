using  System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Quick_FreeRDP.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    public List<string> RdpItems { get; set; } = new()
    {
        "Option 1",
        "Option 2",
        "Option 3"
    };
    
}