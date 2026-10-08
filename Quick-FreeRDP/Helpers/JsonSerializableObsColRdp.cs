using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using Quick_FreeRDP.Helpers;
using Quick_FreeRDP.Models;

[JsonSerializable(typeof(JsonConfigObjectSpec))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class JsonSerializableObsColRdp : JsonSerializerContext
{
}