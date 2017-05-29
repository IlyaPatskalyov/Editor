using Editor.Model;

namespace Editor.Front.DocumentSessions
{
    public class DocumentState
    {
        public Author[] Authors { get; set; }
        public int Revision { get; set; }
        public Operation[] Operations { get; set; }
    }
}