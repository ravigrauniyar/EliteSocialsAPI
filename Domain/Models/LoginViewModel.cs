using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class LoginViewModel
    {
        public string username { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string password { get; set; } = string.Empty;
    }
}
