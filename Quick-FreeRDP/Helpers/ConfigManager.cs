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
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        // Define the config directory (usually ~/.config/YourAppName)
        string configDirectory = Path.Combine(homeDirectory, ".config", "Quick-FreeRDP");

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
            string json = JsonSerializer.Serialize(rdpItems, new JsonSerializerOptions { WriteIndented = true });

            // Write the JSON to the file
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Configuration saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration: {ex.Message}");
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
                returnValue = JsonSerializer.Deserialize<ObservableCollection<RdpItem>>(json)!;
                
                return returnValue;
            }
            else
            {
                Console.WriteLine("Configuration file not found. Returning default configuration.");
                return returnValue;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            return returnValue;
        }
    }
}