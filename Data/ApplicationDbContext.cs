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
        public DbSet<OnlineNotes.Models.Note>? Note { get; set; }
        public DbSet <OnlineNotes.Models.Comment>? Comment { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Note)             // Configure the relationship with Note entity
                .WithMany(n => n.Comments)       // Assuming you have a Comments navigation property in the Note entity
                .HasForeignKey(c => c.NoteId)    // Specify that NoteId is the foreign key for this relationship
                .IsRequired(false);               // Mark it as optional

            // Other configurations...

            base.OnModelCreating(modelBuilder);
        }
    }
}