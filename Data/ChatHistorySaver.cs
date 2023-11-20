using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Models;
using OpenAI_API.Chat;
using System.Security.AccessControl;

namespace OnlineNotes.Data
{
    public sealed class ChatHistorySaver
    {
        private static readonly List<ChatGptMessage> PendingMessages = new List<ChatGptMessage>();
        private static ChatHistorySaver _instance;
        private readonly ApplicationDbContext _context;
        private static readonly object _lock = new object();
        Thread messageSaverToDBThread;
        private const int DbUpdateTimeIntervalMilliseconds = 5000;

        private ChatHistorySaver(ApplicationDbContext context)
        {
            _context = context;
            messageSaverToDBThread = new Thread(SavePendingMessages);
            messageSaverToDBThread.Start();
        }

        public static ChatHistorySaver Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        throw new InvalidOperationException("You must call Initialize() first.");
                    }

                    return _instance;
                }
            }
        }

        public static void Initialize(ApplicationDbContext dbContext)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ChatHistorySaver(dbContext);
                }
                else
                {
                    throw new InvalidOperationException("Already initialized.");
                }
            }
        }

        public void AddMessage(ChatGptMessage message)
        {
            PendingMessages.Add(message);
        }

        public List<ChatGptMessage> GetPendingMessages()
        {
            return new List<ChatGptMessage>(PendingMessages);
        }
        
        public List<ChatGptMessage> getAllChatMessagesFromDb()
        {
            lock (_lock)
            {
                return _context.ChatMessages.ToList();
            }
        }

        private void SavePendingMessages()
        {
            while (true)
            {
                Thread.Sleep(DbUpdateTimeIntervalMilliseconds);
                lock (_lock)
                {
                    foreach (var message in PendingMessages)
                    {
                        _context.ChatMessages.Add(message);
                    }
                    _context.SaveChanges();

                    PendingMessages.Clear();
                }
            }
        }

        public void ClearChatHistory()
        {
            lock (_lock)
            {
                var chatMessages = _context.ChatMessages.ToList();
                if (chatMessages.Count > 0)
                {
                    _context.ChatMessages.RemoveRange(_context.ChatMessages);
                    _context.SaveChanges();
                }
            }
        }
    }
}
