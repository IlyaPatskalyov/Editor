using System;

namespace Editor.Model
{
    internal class Char
    {
        public Char(CharId id, string character, CharId previous, CharId next)
        {
            Id = id;
            Character = character;
            Previous = previous;
            Next = next;
        }

        public readonly CharId Id;
        public readonly string Character;
        public readonly CharId Previous;
        public readonly CharId Next;

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