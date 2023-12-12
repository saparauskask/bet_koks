using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Models;
using OnlineNotes.Models.Quizzes;

namespace OnlineNotes.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Note> Note { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<NoteRating> NoteRating { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ChatGptMessage> ChatMessages { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionOption> QuestionOption { get; set; }
    }
}