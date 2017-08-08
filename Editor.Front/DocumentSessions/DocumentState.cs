namespace Editor.Front.DocumentSessions
{
    public class DocumentState
    {
        public Author[] Authors { get; set; }
        public int Revision { get; set; }
        public string[] Operations { get; set; }
    }

    public class DocumenChange
    {
        public int Position { get; set; }
        public int? Revision { get; set; }
        public string[] Operations { get; set; }
    }
}