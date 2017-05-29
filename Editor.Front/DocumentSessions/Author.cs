using System;

namespace Editor.Front.DocumentSessions
{
    public class Author
    {
        public DateTime LastUpdate { get; set; }
        public Guid ClientId { get; set; }
        public int Position { get; set; }
    }
}