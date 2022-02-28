using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Common.Models
{
    [JsonConverter(typeof(StringEnumConverter))] 
    public enum ResultType
    {
        Property = 1,
        Management = 2,
    }
}