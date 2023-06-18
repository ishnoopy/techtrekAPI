using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace techtrekAPI
{
    public class AuthResponse
    {
        [Key] public int id { get; set; }
        public string email_address { get; set; } = null!;
        public string last_name { get; set; } = null!;
        public string first_name { get; set; } = null!;
        [JsonIgnore] public string password { get; set; } = null!;
        public string token { get; set; } = null!;
        public string role { get; set; } = null!;
    }
}
