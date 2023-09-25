using Microsoft.AspNetCore.Identity;

namespace OnlineNotes.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
    }
}
