using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool saveEnabled = true;

    [ObservableProperty] private RdpItem newRdpItem;

    [ObservableProperty] private RdpItem selectedRdpItem;

    partial void OnNewRdpItemChanged(RdpItem value)
    {
        if (value == null) return;
        
        if (value.Name == NewEntryName)
        {
            SaveEnabled = false;
        }
        else
        {
            SaveEnabled = true;
        }

        value.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(RdpItem.Name))
            {
                SaveEnabled = value.Name != NewEntryName;
            }
        };
    }

    partial void OnSelectedRdpItemChanged(RdpItem value)
    {
        if (value != null)
        {
            // fill out the form based on the selection from the combobox
            NewRdpItem = new RdpItem()
            {
                Name = value.Name,
                IpAddress = value.IpAddress,
                ResolutionHeight = value.ResolutionHeight,
                ResolutionWidth = value.ResolutionWidth,
                FloatBarBool = value.FloatBarBool,
                FullScreenBool = value.FullScreenBool,
            };
        }
    }


    public ObservableCollection<RdpItem> RdpItems { get; set; }

    private const string NewEntryName = "*New Entry*";

    public MainWindowViewModel()
    {
        RdpItems = new ObservableCollection<RdpItem>();

        // Example 1
        RdpItem demoItem = new RdpItem() { };
        demoItem.Name = "Demo";
        demoItem.IpAddress = "127.0.0.1";
        demoItem.ResolutionHeight = 1920;
        demoItem.ResolutionWidth = 1080;
        demoItem.FloatBarBool = false;
        demoItem.FullScreenBool = false;
        RdpItems.Add(demoItem);

        // Example 2
        demoItem = new RdpItem() { };
        demoItem.Name = "Demo 2";
        demoItem.IpAddress = "333.0.0.1";
        demoItem.ResolutionHeight = 1920;
        demoItem.ResolutionWidth = 1080;
        demoItem.FloatBarBool = true;
        demoItem.FullScreenBool = true;
        RdpItems.Add(demoItem);

        // Example 2
        demoItem = new RdpItem() { };
        demoItem.Name = NewEntryName;
        demoItem.IpAddress = string.Empty;
        demoItem.FloatBarBool = true;
        demoItem.FullScreenBool = true;
        RdpItems.Add(demoItem);
    }

    [RelayCommand]
    public void SaveDetails()
    {
        // dialog setup
        Window dialog = new Window
        {
            Title = "Message",
            Content = new TextBlock
            {
                Text = $"Added {SelectedRdpItem.Name}"
            },
            Width = 300,
            Height = 150
        };

        foreach (var rdpItem in RdpItems)
        {
            if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                rdpItem.IpAddress = NewRdpItem.IpAddress;
                rdpItem.ResolutionWidth = NewRdpItem.ResolutionWidth;
                rdpItem.ResolutionHeight = NewRdpItem.ResolutionHeight;
                rdpItem.FloatBarBool = NewRdpItem.FloatBarBool;
                rdpItem.FullScreenBool = NewRdpItem.FullScreenBool;

                dialog.Title = "Updated";
                dialog.Content = new TextBlock() { Text = $"Updated {SelectedRdpItem.Name}" };

                ShowMessageBox(dialog);
                return;
            }
        }

        RdpItem toAdd = new RdpItem();
        toAdd = NewRdpItem;

        RdpItems.Add(toAdd);
        dialog.Title = "Created New";
        dialog.Content = new TextBlock() { Text = $"Created {NewRdpItem.Name}" };

        //NewRdpItem = new RdpItem();

        foreach (var rdpItem in RdpItems)
        {
            if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                SelectedRdpItem = rdpItem; // Switch to the newly created one in the combobox
            }
        }


        ShowMessageBox(dialog);
    }


    private void ShowMessageBox(Window dialog)
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        dialog.ShowDialog(lifetime?.MainWindow!);
    }
}