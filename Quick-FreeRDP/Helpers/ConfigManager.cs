using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.Helpers;

using System;
using System.IO;
using System.Text.Json;

public class ConfigManager
{
    public static string GetConfigFolder()
    {
        // Get the home directory
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Define the config directory (usually ~/.config/YourAppName)
        string configDirectory = Path.Combine(homeDirectory, "Quick-FreeRDP");

        LoggingWithSerilog.Logger($"configDirectory: {configDirectory}");

        // Populate some dummy logs for quickly testing the log window size
        // for(int i = 0; i < 10 ; i++)
        // {
        //     LoggingWithSerilog.Logger($"Example line, example line , does whatever an example line does, can he swing, from a ledge , no he can't , he pig boi");
        // }


        // Ensure the directory exists
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        return configDirectory;
    }

    private static string GetConfigFilePath()
    {
        string configDirectory = GetConfigFolder();

        // Define the config file path
        return Path.Combine(configDirectory, "config.json");
    }

    public static void SaveConfig(
        ObservableCollection<RdpItem> rdpItems,
        ObservableCollection<string> availableResolutions)
    {
        try
        {
            string filePath = GetConfigFilePath();

            var config = new JsonConfigObjectSpec()
            {
                AvailableResolutions = availableResolutions,
                RdpItems = rdpItems
            };

            string json = JsonSerializer.Serialize(config, JsonSerializableObsColRdp.Default.JsonConfigObjectSpec);

            LoggingWithSerilog.Logger("json config Serialized");

            File.WriteAllText(filePath, json);

            LoggingWithSerilog.Logger(
                $"Configuration saved to {filePath}");
        }
        catch (Exception ex)
        {
            LoggingWithSerilog.Logger(
                "Error saving configuration:",
                ex);
        }
    }

    
    public static JsonConfigObjectSpec LoadConfig()
    {
        var returnValue = new JsonConfigObjectSpec();

        try
        {
            string filePath = GetConfigFilePath();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                var config = JsonSerializer.Deserialize(
                    json,
                    JsonSerializableObsColRdp.Default.JsonConfigObjectSpec);

                if (config is not null)
                {
                    return config;
                }

                return returnValue;
            }

            LoggingWithSerilog.Logger(
                "Configuration file not found. Returning default configuration.");

            return returnValue;
        }
        catch (Exception ex)
        {
            LoggingWithSerilog.Logger(
                "Error loading configuration",
                ex);

            return returnValue;
        }
    }
    
    
}