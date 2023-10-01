using Microsoft.AspNetCore.Identity;

namespace OnlineNotes.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string StudentId { get; set; }
    }
}
