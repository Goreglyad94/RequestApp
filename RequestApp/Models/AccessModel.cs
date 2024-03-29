using RequestApp.Domain;
using System.ComponentModel.DataAnnotations;

namespace RequestApp.Models
{
    public class AccessModel
    {
        [Required(ErrorMessage = "Resource is required field")]
        public string Resource { get; set; }
        [Required(ErrorMessage = "Action is required field")]
        public AccessAction Action { get; set; }
    }

    public enum AccessAction
    {
        Grant,
        Deny
    }
}
