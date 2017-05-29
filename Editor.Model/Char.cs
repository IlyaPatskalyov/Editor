using System;

namespace Editor.Model
{
    public class Char
    {
        public Char()
        {
        }

        public Char(CharId id, string character, CharId previous, CharId next)
        {
            Id = id;
            Character = character;
            Previous = previous;
            Next = next;
        }

        public CharId Id { get; set; }
        public string Character { get; set; }
        public CharId Previous { get; set; }
        public CharId Next { get; set; }

        public static Char Begin
        {
            get
            {
                var id = new CharId(Guid.Empty, 0);
                return new Char(id, "", id, id);
            }
        }

        public static Char End
        {
            get
            {
                var id = new CharId(Guid.Empty, 1);
                return new Char(id, "", id, id);
            }
        }
    }
}