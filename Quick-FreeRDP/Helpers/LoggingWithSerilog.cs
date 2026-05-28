using System;
using System.Threading.Tasks;
using Quick_FreeRDP.ViewModels;
using Serilog;
using Avalonia.Threading;

namespace Quick_FreeRDP.Helpers;

public static class LoggingWithSerilog
{
    private static Action<string>? _uiLogger;

    public static void SetUiLogger(Action<string> uiLogger)
    {
        _uiLogger = uiLogger;
    }


    public static void LoggingWithSerilogStart()
    {
        string LogPath =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Quick-FreeRDP",
                "Log.txt");

        // Serilog Setup
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(LogPath, outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("Started Quick-FreeRDP");
    }

    public static void Logger(string text, Exception? ex = null, bool errorPopup = false)
    {
        Log.Information(text);

        if (ex != null)
        {
            Log.Error(Convert.ToString(ex)!);
        }

        // 👇 update UI console
        _uiLogger?.Invoke(text);

        if (ex != null)
            _uiLogger?.Invoke(ex.ToString());

        if (errorPopup)
        {
            MessageBox msg = new MessageBox();
            msg.Message = text;
            msg.Title = "Error";

            msg.Show();
        }
    }
}