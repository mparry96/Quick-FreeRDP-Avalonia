using System;
// using Avalonia.Logging;
using Serilog;
using Serilog.Events;

namespace Quick_FreeRDP.Helpers;

public class LoggingWithSerilog
{
    public static void LoggingWithSerilogStart()
    {
        string LogPath =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Quick-FreeRDP",
                "Log.txt");

        // Serilog Setup
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(LogPath,LogEventLevel.Verbose,
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("Started Quick-FreeRDP");
    }

    public static void Logger(string text, Exception? ex = null)
    {
        Log.Information(text);

        if (ex != null)
        {
            Log.Error(Convert.ToString(ex));
        }
    }
    
    
}