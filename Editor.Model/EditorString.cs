using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Model
{
    public class EditorString : IEditorString
    {
        private readonly Guid clientId;
        private readonly List<Char> chars;
        private readonly Dictionary<CharId, bool> deletedIds;
        private int operationId = 0;

        public EditorString(Guid? clientId = null)
        {
            this.clientId = clientId ?? Guid.NewGuid();
            chars = new List<Char> {Char.Begin, Char.End};
            deletedIds = new Dictionary<CharId, bool>(CharId.Comparer);
        }

        public Operation[] GenerateOperations()
        {
            return chars.Skip(1).Take(chars.Count - 2).Select(c => new Operation(OperationType.Insert, c)).ToArray();
        }

        public Operation[] GenerateOperations(string newValue)
        {
            var oldValue = ToString();
            int start = 0;
            while (start < oldValue.Length && start < newValue.Length && oldValue[start] == newValue[start])
                start++;

            if (oldValue.Length == newValue.Length && start == newValue.Length)
                return new Operation[0];

            int endOld = oldValue.Length - 1, endNew = newValue.Length - 1;
            while ((endOld >= start) && (endNew >= start) && oldValue[endOld] == newValue[endNew])
            {
                endOld--;
                endNew--;
            }

            var result = new List<Operation>();

            if (endNew <= endOld)
            {
                for (var i = start; i <= endOld; i++)
                    result.Add(GenerateDeleteOperation(start));
                endOld = start - 1;
            }

            if (endNew > endOld)
                for (var i = start; i <= endNew; i++)
                    result.Add(GenerateInsertOperation(newValue[i], i));
            return result.ToArray();
        }

        public void ApplyOperations(IEnumerable<Operation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation.OperationType == OperationType.Insert)
                    Insert(operation.Char);
                if (operation.OperationType == OperationType.Delete)
                    Delete(operation.Char);
            }
        }

        private Operation GenerateInsertOperation(char ch, int position)
        {
            var previous = this[position];
            var next = this[position + 1];

            var newChar = new Char(new CharId(clientId, operationId++), ch.ToString(), previous.Id, next.Id);
            Insert(newChar);
            return new Operation(OperationType.Insert, newChar);
        }

        private Operation GenerateDeleteOperation(int position)
        {
            var toDelete = this[position + 1];
            Delete(toDelete);
            return new Operation(OperationType.Delete, toDelete);
        }

        private void Delete(Char toDelete)
        {
            deletedIds.Add(toDelete.Id, true);
        }

        public Char this[int i]
        {
            get
            {
                return chars.Where(t => !deletedIds.ContainsKey(t.Id))
                            .Skip(i)
                            .First();
            }
        }

        private void Insert(Char newChar)
        {
            var indexById = new Dictionary<CharId, int>(CharId.Comparer);
            for (var i = 0; i < chars.Count; i++)
                indexById[chars[i].Id] = i;
            Insert(newChar, newChar.Previous, newChar.Next, indexById);
        }

        private void Insert(Char newChar, CharId previousId, CharId nextId, Dictionary<CharId, int> indexById)
        {
            int nextIndex, previousIndex;
            if (!(indexById.TryGetValue(previousId, out previousIndex) &&
                  indexById.TryGetValue(nextId, out nextIndex) &&
                  nextIndex >= previousIndex))
                throw new ArgumentException("Wrong next or previous");

            if (nextIndex == previousIndex + 1)
                chars.Insert(nextIndex, newChar);
            else
            {
                var substring = chars.GetRange(previousIndex + 1, nextIndex - previousIndex);
                var skipped = substring
                    .SkipWhile(l => CompareChars(newChar, l) < 0)
                    .Take(2)
                    .ToList();
                if (skipped.Count < 2)
                    skipped = substring;
                Insert(newChar, skipped[0].Id, skipped[1].Id, indexById);
            }
        }

        private static int CompareChars(Char newChar, Char l)
        {
            if (l.Id.ClientId != newChar.Id.ClientId)
                return string.Compare(l.Id.ClientId, newChar.Id.ClientId, StringComparison.Ordinal);
            return l.Id.OperationId.CompareTo(newChar.Id.OperationId);
        }


        public override string ToString()
        {
            return string.Concat(chars.Where(t => !deletedIds.ContainsKey(t.Id)).Select(t => t.Character));
        }
    }
}