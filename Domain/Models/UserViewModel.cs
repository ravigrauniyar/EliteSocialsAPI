using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class UserViewModel
    {
        public Guid userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string email { get; set; } = null;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public bool isTFAEnabled { get; set; }
    }
}
