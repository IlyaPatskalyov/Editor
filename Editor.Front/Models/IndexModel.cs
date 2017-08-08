using System;
using Editor.Storage;

namespace Editor.Front.Models
{
    public class IndexModel
    {
        public Guid UserId { get; set; }
        public Document[] Documents { get; set; }
    }
}