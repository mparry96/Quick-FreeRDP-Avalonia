using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

    [ObservableProperty] private string? rdpPassword;

    [ObservableProperty] private string windowConsoleLog = "Ready";

    [ObservableProperty] private string resolutionX = string.Empty;

    [ObservableProperty] private string resolutionY = string.Empty;

    [ObservableProperty] private ObservableCollection<string> resolutionsListboxItems = [];
    
    [ObservableProperty] private string? selectedComboboxResolution = null;

    [ObservableProperty] private bool presetResolutionsEnabled = false; // only show temporarily when toggle button is used
    
    partial void OnSelectedComboboxResolutionChanged(string? value)
    {
            PresetResolutionsEnabled = false;

            if (value == null || !value.Contains("x")) return;

            var x = Convert.ToInt32(value.Split("x")[0]);
            var y =  Convert.ToInt32(value.Split("x")[1]);
            NewRdpItem.ResolutionWidth = x;
            NewRdpItem.ResolutionHeight = y;
            
            // fix for ComboBox immediately reapplying the selected value
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                SelectedComboboxResolution = string.Empty;
            });
        
    }

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
                Domain = string.Empty,
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
            Domain = value.Domain,
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
        if (string.IsNullOrEmpty((value)))
        {
            return;
        }

        ScrollToEndRequested?.Invoke();
    }

    public event Action? ScrollToEndRequested;

    public MainWindowViewModel()
    {
        LoggingWithSerilog.LoggingWithSerilogStart();


        LoggingWithSerilog.SetUiLogger(AppendLog);

        NewRdpItem = new RdpItem();

        RdpItems = [];
        
        var configFromJson = ConfigManager.LoadConfig();
        
        
        RdpItems = configFromJson.RdpItems;
        ResolutionsListboxItems = configFromJson.AvailableResolutions;
        

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
    public async Task Launch()
    {
        if (string.IsNullOrEmpty(RdpPassword))
        {
            LoggingWithSerilog.Logger("Error, a password must be provided", null, true);
            return;
        }

        int errorCount = 0;

        var startInfo = new ProcessStartInfo { };

        try
        {
            var args = new List<string>
            {
                $"/v:{NewRdpItem.IpAddress}"
            };

            args.Add($"/size:{NewRdpItem.ResolutionWidth}x{NewRdpItem.ResolutionHeight}");
            
            args.Add($"/u:{NewRdpItem.UserName}");
            args.Add($"/d:{NewRdpItem.Domain}"); // stdin expects an empty domain to be provided, even when a domain is not used

            args.Add("/cert:ignore");
            args.Add("/from-stdin");


            if (NewRdpItem.FullScreenBool)
                args.Add("/f");

            if (NewRdpItem.FloatBarBool)
                args.Add("/floatbar:show:always");

            string xfreerdpPath = string.Empty;

            // These are BOTH for freerdp versions built manually from the .tar.bz2 source, and included in my flatpak via the com.mparry96.QuickRDP.json
            if (File.Exists("/app/bin/xfreerdp3"))
            {
                xfreerdpPath = "/app/bin/xfreerdp3";
            }
            else if (File.Exists("/app/bin/xfreerdp"))
            {
                xfreerdpPath = "/app/bin/xfreerdp";
            }

            else
            {
                // Rider / local machine fallback
                startInfo.FileName = "flatpak";
                startInfo.ArgumentList.Add("run");
                startInfo.ArgumentList.Add("--command=xfreerdp");
                startInfo.ArgumentList.Add("com.freerdp.FreeRDP");
            }


            if (!string.IsNullOrEmpty(xfreerdpPath))
            {
                startInfo.FileName = xfreerdpPath;
            }

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;


            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            LoggingWithSerilog.Logger(
                $"{xfreerdpPath} {string.Join(" ", startInfo.ArgumentList)}");

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

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
                    Interlocked.Increment(ref errorCount);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.StandardInput.WriteLineAsync(RdpPassword);
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();

            // Optional:
            // await process.WaitForExitAsync();
            //
            // if (errorCount > 0)
            // {
            //     LoggingWithSerilog.Logger(
            //         $"{errorCount} Errors launching RDP session, see log",
            //         null,
            //         true);
            // }
        }
        catch (Exception e)
        {
            LoggingWithSerilog.Logger(
                "Error caught launching RDP session, see log",
                e,
                true);

            throw;
        }
    }

    [RelayCommand]
    public void AddResolution()
    {
        if (string.IsNullOrEmpty(ResolutionX) || string.IsNullOrEmpty(ResolutionY))
            return;


        if (int.TryParse(ResolutionX, out int number) && number > 0
                                                      && int.TryParse(ResolutionY, out int number2) && number2 > 0)
        {
            ResolutionsListboxItems.Add($"{number}x{number2}");
            
            // Sort by resolution width first, then height as 2nd priority
            var sorted = ResolutionsListboxItems
                .Distinct()
                .OrderBy(x => int.Parse(x.Split('x')[0]))   // width
                .ThenBy(x => int.Parse(x.Split('x')[1]));   // height

            ResolutionsListboxItems = new ObservableCollection<string>(sorted);
            
            ConfigManager.SaveConfig(RdpItems, ResolutionsListboxItems);
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
            ConfigManager.SaveConfig(RdpItems, ResolutionsListboxItems);
        }

        if (RdpItems.Any())
        {
            // SelectedRdpItem = RdpItems.First();
            SelectedRdpItem = RdpItems[0];
        }
    }

    [RelayCommand]
    public void ConfigFolder()
    {
        var configLocationFolder = ConfigManager.GetConfigFolder();

        Process.Start(new ProcessStartInfo
        {
            FileName = configLocationFolder,
            UseShellExecute = true
        });
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
                rdpItem.Domain = NewRdpItem.Domain;
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


        ConfigManager.SaveConfig(RdpItems , ResolutionsListboxItems);

        await msg.ShowAsync();
    }
}