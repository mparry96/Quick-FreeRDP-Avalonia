using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quick_FreeRDP.Helpers;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool deleteAndLaunchEnabled = true;
    
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
            DeleteAndLaunchEnabled = false;
        }
        else
        {
            SaveEnabled = true;
            DeleteAndLaunchEnabled = true;
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
                UserName = value.UserName,
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
        LoggingWithSerilog.LoggingWithSerilogStart();
        
        NewRdpItem = new RdpItem();

        RdpItems = new ObservableCollection<RdpItem>();

        RdpItems = ConfigManager.LoadConfig();
        
        
        if (!RdpItems.Any())
        {
            var newEntryOption = new RdpItem() { };
            newEntryOption = new RdpItem() { };
            newEntryOption.Name = NewEntryName;
            newEntryOption.IpAddress = string.Empty;
            newEntryOption.FloatBarBool = true;
            newEntryOption.FullScreenBool = true;
            RdpItems.Add(newEntryOption);
        }
        
        SelectedRdpItem = RdpItems[0];
  

        SortHelper.SortByName(RdpItems);

   
    }

    [RelayCommand]
    public void Launch()
    {
        try
        {
            string fullScreenCommand = SelectedRdpItem.FullScreenBool ? "/f" : string.Empty;
            string floatbarCommand = SelectedRdpItem.FloatBarBool ? "/floatbar:show:always" : string.Empty;
            // Process.Start(new ProcessStartInfo
            // {
            //     FileName = "gnome-terminal", // or use xterm, konsole, etc.
            //     Arguments =
            //         $"-e \"xfreerdp /v:{SelectedRdpItem.IpAddress} /u:{SelectedRdpItem.UserName} {fullScreenCommand} {floatbarCommand} /size:{SelectedRdpItem.ResolutionWidth}x{SelectedRdpItem.ResolutionHeight}\"",
            //     UseShellExecute = false // Needed to pass the command to the terminal
            // });
            
            Process.Start(new ProcessStartInfo
            {
                FileName = "gnome-terminal",
                Arguments =
                    $"-- bash -c \"xfreerdp /v:{SelectedRdpItem.IpAddress} /u:{SelectedRdpItem.UserName} {fullScreenCommand} {floatbarCommand} /size:{SelectedRdpItem.ResolutionWidth}x{SelectedRdpItem.ResolutionHeight}; exec bash\"",
                UseShellExecute = false
            });
        }
        catch (Exception e)
        {
            LoggingWithSerilog.Logger("Error launching rdp session via terminal",e);
            throw;
        }
    }

    [RelayCommand]
    public async Task Delete()
    {
        int removeIndex = 0;
        foreach (var rdpItem in RdpItems)
        {
            if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                removeIndex =  RdpItems.IndexOf(rdpItem);
            }
        }

        if (removeIndex > 0)
        {
            RdpItems.RemoveAt(removeIndex);
            ConfigManager.SaveConfig(RdpItems);
        }

        if (RdpItems.Any())
        {
            // SelectedRdpItem = RdpItems.First();
            SelectedRdpItem = RdpItems[0];
        }
    }

    [RelayCommand]
    public async Task SaveDetails()
    {
        MessageBox msg = new MessageBox();
        bool updated = false;

        RdpItem switchToThisOne = new RdpItem();

        if (RdpItems.Count > 0)
        {
            switchToThisOne = RdpItems.First(); // a fallback item only
        }

        foreach (var rdpItem in RdpItems)
        {
            if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                rdpItem.IpAddress = NewRdpItem.IpAddress;
                rdpItem.UserName = NewRdpItem.UserName;
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


        ConfigManager.SaveConfig(RdpItems);

        await msg.Show();
    }
}