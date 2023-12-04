using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Models;

namespace OnlineNotes.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<OnlineNotes.Models.Note> Note { get; set; }
        public DbSet <OnlineNotes.Models.Comment> Comment { get; set; }
        public DbSet <OnlineNotes.Models.NoteRating> NoteRating { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ChatGptMessage> ChatMessages { get; set; }
        public DbSet<NoteAttachment> NoteAttachments { get; set; }
    }
}