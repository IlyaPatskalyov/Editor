using System;

namespace Editor.Storage
{
    public class Document
    {
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }
    }
}