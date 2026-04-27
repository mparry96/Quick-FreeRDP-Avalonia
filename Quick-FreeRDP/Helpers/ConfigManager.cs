using System.Collections.ObjectModel;
using Quick_FreeRDP.Models;

namespace Quick_FreeRDP.Helpers;

using System;
using System.IO;
using System.Text.Json;

public class ConfigManager
{
    private static string GetConfigFilePath()
    {
        // Get the home directory
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        
        // Define the config directory (usually ~/.config/YourAppName)
        string configDirectory = Path.Combine(homeDirectory, "Quick-FreeRDP");
        
        LoggingWithSerilog.Logger($"configDirectory: {configDirectory}");
  

        // Ensure the directory exists
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        // Define the config file path
        return Path.Combine(configDirectory, "config.json");
    }

    public static void SaveConfig(ObservableCollection<RdpItem> rdpItems)
    {
        try
        {
            string filePath = GetConfigFilePath();
            
            // Serialize the config object to JSON
            string json = JsonSerializer.Serialize(rdpItems, JsonSerializableObsColRdp.Default.ObservableCollectionRdpItem);
            
            LoggingWithSerilog.Logger($"json config Serialized");

            // Write the JSON to the file
            File.WriteAllText(filePath, json);
            LoggingWithSerilog.Logger($"Configuration saved to {filePath}");
        }
        catch (Exception ex)
        {
            LoggingWithSerilog.Logger($"Error saving configuration:" , ex);
        }
    }

    public static ObservableCollection<RdpItem> LoadConfig()
    {
        ObservableCollection<RdpItem> returnValue = [];
        try
        {
            string filePath = GetConfigFilePath();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the JSON file and deserialize into the object
                string json = File.ReadAllText(filePath);
                returnValue = JsonSerializer.Deserialize(
                    json,
                    JsonSerializableObsColRdp.Default.ObservableCollectionRdpItem
                )!;
                
                return returnValue;
            }
            else
            {
                LoggingWithSerilog.Logger("Configuration file not found. Returning default configuration.");
                return returnValue;
            }
        }
        catch (Exception ex)
        {
            LoggingWithSerilog.Logger($"Error loading configuration", ex);
            return returnValue;
        }
    }
}