using Newtonsoft.Json.Converters;
using RequestApp.Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RequestApp.Models
{
    public class RequestResponse
    {
        [JsonPropertyName("resource")]
        public string? Resource { get; set; }
        [JsonPropertyName("decision")]
        public RequestStatus Decision { get; set; }
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
