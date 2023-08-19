using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class UserEntity
    {
        [Key]
        public Guid userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
