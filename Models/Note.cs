﻿using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; private set; }
        public string Contents { get; set; }
        // Collection navigation containing dependents
        public ICollection<Comment> Comments { get; } = new List<Comment>();
        public NoteStatus Status { get; set; }

        public void SetTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                Title = title;
            }
            
        }
    }
}
