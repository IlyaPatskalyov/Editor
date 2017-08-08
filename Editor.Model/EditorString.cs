using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Model
{
    public class EditorString : IEditorString
    {
        private readonly CharCollection chars;
        private uint operationId;

        public EditorString(Guid? clientId = null)
        {
            operationId = new CharId(clientId ?? Guid.NewGuid(), 0).Value;
            chars = new CharCollection();
            chars.Insert(0, new[]
                            {
                                Char.Begin,
                                Char.End
                            });
        }

        public string[] GenerateOperations(string newValue)
        {
            var oldValue = chars.BuildString();
            var start = 0;
            while (start < oldValue.Length && start < newValue.Length && oldValue[start] == newValue[start]) start++;

            if (chars.Length - 2 == newValue.Length && start == newValue.Length)
                return new string[0];

            int endOld = chars.Length - 3, endNew = newValue.Length - 1;

            while (endOld >= start && endNew >= start && oldValue[endOld] == newValue[endNew])
            {
                endOld--;
                endNew--;
            }

            var toDelete = new Char[0];
            var toInsert = new Char[0];

            if (endNew <= endOld)
            {
                toDelete = GenerateDeleteOperation(start, endOld);
                ApplyDeleteOperations(toDelete);
                endOld = start - 1;
            }

            if (endNew > endOld)
            {
                toInsert = GenerateInsertOperation(newValue.Substring(start, endNew - start + 1), start);
                ApplyInsertOperations(toInsert);
            }
            return
                toDelete.Select(c => new Operation(OperationType.Delete, c))
                        .Concat(toInsert.Select(c => new Operation(OperationType.Insert, c)))
                        .Select(OperationSerializer.Serialize)
                        .FastToArray(toDelete.Length + toInsert.Length);
        }

        public void ApplyOperations(ICollection<string> ops)
        {
            var operations = ops.Select(OperationSerializer.Deserialize).ToArray();
            var countInsert = operations.Count(t => t.OperationType == OperationType.Insert);
            ApplyInsertOperations(operations.Where(t => t.OperationType == OperationType.Insert).Select(t => t.Char).FastToArray(countInsert));
            ApplyDeleteOperations(operations.Where(t => t.OperationType == OperationType.Delete).Select(t => t.Char)
                                            .FastToArray(ops.Count - countInsert));
        }

        private void ApplyDeleteOperations(Char[] toDelete)
        {
            chars.Delete(toDelete);
        }

        private void ApplyInsertOperations(Char[] toInsert)
        {
            if (toInsert.Length > 0)
            {
                Char[] batch;
                var first = 0;
                for (var i = 1; i < toInsert.Length; i++)
                {
                    if (!(toInsert[i].Previous.Value == toInsert[i - 1].Id.Value && toInsert[i].Next.Value == toInsert[i - 1].Next.Value))
                    {
                        batch = toInsert.Skip(first).Take(i - first).FastToArray(i - first);
                        chars.Insert(SearchPosition(batch[0].Id, batch[0].Previous, batch[0].Next), batch);
                        first = i;
                    }
                }
                batch = first == 0 ? toInsert : toInsert.Skip(first).Take(toInsert.Length - first).FastToArray(toInsert.Length - first);
                chars.Insert(SearchPosition(batch[0].Id, batch[0].Previous, batch[0].Next), batch);
            }
        }

        private Char[] GenerateInsertOperation(string s, int position)
        {
            var previousId = chars.GetByVisiblePosition(position).Id;
            var nextId = chars.GetByVisiblePosition(position + 1).Id;

            var result = new Char[s.Length];
            for (var i = 0; i < s.Length; i++)
            {
                var currentId = new CharId(operationId++);
                result[i] = new Char(currentId, s[i].ToString(), previousId, nextId);
                previousId = currentId;
            }
            return result;
        }

        private Char[] GenerateDeleteOperation(int from, int to)
        {
            var result = new Char[to - from + 1];
            for (var i = from; i <= to; i++)
                result[i - from] = chars.GetByVisiblePosition(i + 1);
            return result;
        }

        private int SearchPosition(CharId newCharId, CharId previousId, CharId nextId)
        {
            int nextIndex, previousIndex;
            if (!(chars.TryGetPositionById(previousId, out previousIndex) &&
                  chars.TryGetPositionById(nextId, out nextIndex) &&
                  nextIndex >= previousIndex))
                throw new ArgumentException("Wrong next or previous");

            if (nextIndex == previousIndex + 1)
            {
                return nextIndex;
            }

            var skipped = chars
                .GetChars()
                .Skip(previousIndex)
                .Take(nextIndex - previousIndex)
                .SkipWhile(l => CharId.Compare(newCharId, l.Id) < 0)
                .Take(2)
                .ToList();
            if (skipped.Count < 2)
                skipped = chars.GetChars().Skip(previousIndex).Take(2).ToList();
            return SearchPosition(newCharId, skipped[0].Id, skipped[1].Id);
        }

        public override string ToString()
        {
            return chars.BuildString();
        }
    }
}