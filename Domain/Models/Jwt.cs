namespace Domain.Models
{
    public class Jwt
    {
        public string key { get; set; } = string.Empty;
        public string issuer { get; set; } = string.Empty;
        public string audience { get; set; } = string.Empty;
    }
}
