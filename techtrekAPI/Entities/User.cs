using System.ComponentModel.DataAnnotations;

namespace techtrekAPI.Entities
{
    public class User
    {
        [Key] public int id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? email_address { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
    }
}
