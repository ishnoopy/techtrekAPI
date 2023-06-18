using Microsoft.AspNetCore.Identity;

namespace techtrekAPI.Helpers
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }
    }
}
