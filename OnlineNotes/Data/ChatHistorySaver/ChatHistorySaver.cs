using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Models;
using OpenAI_API.Chat;
using System.Collections.Concurrent;
using System.Security.AccessControl;

namespace OnlineNotes.Data.ChatHistorySaver
{
    public sealed class ChatHistorySaver : IChatHistorySaver
    {
        private static readonly ConcurrentQueue<ChatGptMessage> PendingMessages = new ConcurrentQueue<ChatGptMessage>();
        private static ChatHistorySaver? _instance;
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
                    throw new InvalidOperationException("Already initializeeed.");
                }
            }
        }

        public void AddMessage(ChatGptMessage message)
        {
            PendingMessages.Enqueue(message);
        }

        public List<ChatGptMessage> GetPendingMessages()
        {
            return new List<ChatGptMessage>(PendingMessages.ToList());
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
                _context.ChatMessages.RemoveRange(chatMessages);
                _context.SaveChanges();
            }
        }
    }
}
