using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //public RdpItem SelectedRdpItem { get; set; }
    
    [ObservableProperty]
    private RdpItem selectedRdpItem;
    
    public List<RdpItem> RdpItems { get; set; }

    public MainWindowViewModel()
    {
       // SelectedRdpItem = new RdpItem();

        RdpItems = new List<RdpItem>();

        RdpItem demoItem = new RdpItem()
        {
            Name = "Demo",
            IpAddress = "127.0.0.1",
            ResolutionHeight = 1920,
            ResolutionWidth = 1080,
            FloatBarBool = false,
            FullScreenBool = false
        };

        RdpItems.Add(demoItem);
    }

    [RelayCommand]
    public void SaveDetails()
    {
        var dialog = new Window
        {
            Title = "Title",
            Content = new TextBlock
            {
                Text = $"RdpName: {SelectedRdpItem.Name} , IpAddress: {SelectedRdpItem.IpAddress}"
            },
            Width = 300,
            Height = 150
        };

        var lifetime = Application.Current?.ApplicationLifetime
            as IClassicDesktopStyleApplicationLifetime;

        dialog.ShowDialog(lifetime?.MainWindow!);

        //   IApplicationLifetime owner = Application.Current?.ApplicationLifetime! as IApplicationLifetime;

        // dialog.ShowDialog(owner.MainWindow);
    }
}