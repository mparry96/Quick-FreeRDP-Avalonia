using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quick_FreeRDP.Helpers;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial bool DeleteAndLaunchEnabled { get; set; } = true;
    [ObservableProperty] public partial bool SaveEnabled { get; set; } = true;
    [ObservableProperty] public partial RdpItem NewRdpItem { get; set; }
    [ObservableProperty] public partial RdpItem SelectedRdpItem { get; set; }

    [ObservableProperty] public partial string RdpPassword { get; set; }

    [ObservableProperty] public partial string WindowConsoleLog { get; set; } = "Ready";

    partial void OnNewRdpItemChanged(RdpItem value)
    {
        //  if (value == null) return;

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
        // value really is null sometimes, keep this check
        // caused by how bindings & collection updates work
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value == null)
        {
            NewRdpItem = new RdpItem()
            {
                Name = string.Empty,
                IpAddress = string.Empty,
                UserName = string.Empty,
                FloatBarBool = true,
                FullScreenBool = false,
            };
            return;
        }

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


    public ObservableCollection<RdpItem> RdpItems { get; set; }

    private const string NewEntryName = "*New Entry*";

    private void AppendLog(string message)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => { WindowConsoleLog += Environment.NewLine + message; });
    }

    partial void OnWindowConsoleLogChanged(string value)
    {
        ScrollToEndRequested?.Invoke();
    }

    public event Action? ScrollToEndRequested;

    public MainWindowViewModel()
    {
        LoggingWithSerilog.LoggingWithSerilogStart();


        LoggingWithSerilog.SetUiLogger(AppendLog);

        NewRdpItem = new RdpItem();

        RdpItems = [];

        RdpItems = ConfigManager.LoadConfig();


        if (!RdpItems.Any())
        {
            var newEntryOption = new RdpItem
            {
                Name = NewEntryName,
                IpAddress = string.Empty,
                FloatBarBool = true,
                FullScreenBool = true
            };
            RdpItems.Add(newEntryOption);
        }

        SelectedRdpItem = RdpItems[0];
        SortHelper.SortByName(RdpItems);
    }

    [RelayCommand]
    public void Launch()
    {
        int errorCount = 0;
        try
        {
            var args = new List<string>
            {
                $"/v:{SelectedRdpItem.IpAddress}",
                $"/u:{SelectedRdpItem.UserName}",
                $"/size:{SelectedRdpItem.ResolutionWidth}x{SelectedRdpItem.ResolutionHeight}",

                // Ignore self-signed cert prompts
                "/cert:ignore",

                // Read password securely from stdin
                "/from-stdin"
            };

            if (SelectedRdpItem.FullScreenBool)
                args.Add("/f");

            if (SelectedRdpItem.FloatBarBool)
                args.Add("/floatbar:show:always");

            string xfreerdpPath =
                File.Exists("/app/bin/xfreerdp")
                    ? "/app/bin/xfreerdp"
                    : "xfreerdp";

            var startInfo = new ProcessStartInfo
            {
                FileName = xfreerdpPath,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            var process = Process.Start(startInfo);

            if (process != null)
            {
                process.OutputDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                        LoggingWithSerilog.Logger(e.Data);
                };

                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        LoggingWithSerilog.Logger(e.Data);
                        errorCount += 1;
                    }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Send password securely via stdin
                process.StandardInput.WriteLine(RdpPassword);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                
                process.WaitForExit();
                if (errorCount > 0)
                {
                    LoggingWithSerilog.Logger($"{errorCount} Errors launching RDP session, see log",null, true);
                }
            }
        }
        catch (Exception e)
        {
            LoggingWithSerilog.Logger("Error caught launching RDP session, see log", e, true);
            throw;
        }
        
 
    }


    [RelayCommand]
    public void Delete()
    {
        int removeIndex = 0;
        foreach (var rdpItem in RdpItems)
        {
            if (string.Equals(rdpItem.Name, NewRdpItem.Name, StringComparison.OrdinalIgnoreCase))
            {
                removeIndex = RdpItems.IndexOf(rdpItem);
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
            RdpItem toAdd = NewRdpItem;

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

        await msg.ShowAsync();
    }
}