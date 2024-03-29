using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RequestApp.Models
{
    public class RequestModel
    {
        [Required(ErrorMessage = "Resource is required field")]
        public string? Resource { get; set; }
    }
}
