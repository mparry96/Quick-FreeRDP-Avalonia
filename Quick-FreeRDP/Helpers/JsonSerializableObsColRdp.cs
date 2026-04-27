namespace Quick_FreeRDP.Helpers;

using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using Quick_FreeRDP.Models;

[JsonSerializable(typeof(ObservableCollection<RdpItem>))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class JsonSerializableObsColRdp : JsonSerializerContext
{
    
}