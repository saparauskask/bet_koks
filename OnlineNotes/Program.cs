using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Data.ChatHistorySaver;
using OnlineNotes.Middleware;
using OnlineNotes.Interceptors;
using OnlineNotes.Models;
using OnlineNotes.Services.CommentsServices;
using OnlineNotes.Services.NotesServices;
using OnlineNotes.Services.OpenAIServices;
using OnlineNotes.Services.QuizzesServices;
using OnlineNotes.Services.RatingServices;
using Serilog;
using Serilog.Events;

namespace OnlineNotes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddSingleton<UpdateAudiatbleEntities>();

            builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    var auditInterceptor = sp.GetService<UpdateAudiatbleEntities>()!;

                    options.UseSqlServer(connectionString).AddInterceptors(auditInterceptor);
                });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Enable session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpClient();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IOpenAIService, OpenAIService>();
            builder.Services.AddScoped<IChatBotService, ChatBotService>();
            builder.Services.AddScoped<ICommentsService, CommentsService>();
            builder.Services.AddScoped<INotesService, NotesService>();
            builder.Services.AddScoped<INoteRatingService, NoteRatingService>();
            builder.Services.AddScoped<ReferencesRepository>();
            builder.Services.AddScoped<IQuizzesService, QuizzesService>();
            builder.Services.AddScoped<IQuizGeneratorService, QuizGeneratorService>();

            var serviceProvider = builder.Services.BuildServiceProvider();
            ChatHistorySaver.Initialize(serviceProvider.GetRequiredService<ApplicationDbContext>());

            // logger configuration for writting log messages to a file
            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            builder.Logging.AddSerilog(logger);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Notes/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<RequestMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Notes}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}