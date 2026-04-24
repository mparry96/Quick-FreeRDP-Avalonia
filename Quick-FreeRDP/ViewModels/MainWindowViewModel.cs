using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quick_FreeRDP.Helpers;
using Quick_FreeRDP.Models;
using Tmds.DBus.Protocol;

namespace Quick_FreeRDP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool saveEnabled = true;

    [ObservableProperty] private RdpItem newRdpItem;

    [ObservableProperty] private RdpItem selectedRdpItem;

    partial void OnNewRdpItemChanged(RdpItem value)
    {
        if (value == null) return;

        // Check between combo box switches if button needs disabling
        if (value.Name == NewEntryName)
        {
            SaveEnabled = false;
        }
        else
        {
            SaveEnabled = true;
        }

        // Enable or disable as keyboard input is changed
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
        
        SortHelper.SortByName(RdpItems);

        if (RdpItems.Any())
        {
           // SelectedRdpItem = RdpItems.First();
           SelectedRdpItem = RdpItems[1];
        }
     
    }

    [RelayCommand]
    public async Task SaveDetails()
    {
        MessageBox msg = new MessageBox();
        bool updated = false;
        
        RdpItem switchToThisOne = new  RdpItem();
        switchToThisOne = RdpItems.First(); // a fallback item only

        foreach (var rdpItem in RdpItems)
        {
            if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                rdpItem.IpAddress = NewRdpItem.IpAddress;
                rdpItem.ResolutionWidth = NewRdpItem.ResolutionWidth;
                rdpItem.ResolutionHeight = NewRdpItem.ResolutionHeight;
                rdpItem.FloatBarBool = NewRdpItem.FloatBarBool;
                rdpItem.FullScreenBool = NewRdpItem.FullScreenBool;

                updated = true;
                switchToThisOne = rdpItem;
                
                msg.Title = "Updated";
                msg.Message = $"Updated {SelectedRdpItem.Name}";
            }
        }

        if (!updated)
        {
            RdpItem toAdd = new RdpItem();
            toAdd = NewRdpItem;

            RdpItems.Add(toAdd);
            msg.Title = "Created New";
            msg.Message = $"Created {NewRdpItem.Name}";


            foreach (var rdpItem in RdpItems)
            {
                if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
                {
                    switchToThisOne = rdpItem; // Switch to the newly created one in the combobox
                }
            }
        }
    
        SortHelper.SortByName(RdpItems);
        SelectedRdpItem = switchToThisOne;

        await msg.Show();
    }


    

}