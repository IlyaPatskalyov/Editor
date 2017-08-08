using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.Model
{
    internal class CharCollection
    {
        private const int MaxBlockSize = 5000;
        private readonly Dictionary<CharId, CharElement> idToChar;
        private List<CharBlock> blocks;

        public CharCollection()
        {
            blocks = new List<CharBlock>()
                     {
                         new CharBlock
                         {
                             Elements = new CharElement[0],
                             VisibleElements = new CharElement[0]
                         }
                     };


            idToChar = new Dictionary<CharId, CharElement>(CharId.Comparer);
        }

        private class CharBlock
        {
            public int Id;
            public int BlockPosition;
            public int BlockVisiblePosition;
            public CharElement[] Elements;
            public CharElement[] VisibleElements;
        }

        private class CharElement
        {
            public int Position;
            public int VisiblePosition;
            public Char Char;
            public CharBlock Block;
        }

        public int Length { get; private set; }

        public void Insert(int position, Char[] newChars)
        {
            Length += newChars.Length;

            var blockId = blocks.Search(new CharBlock
                                        {
                                            BlockPosition = position
                                        }, blockPositionComparer);
            var block = blocks[blockId];
            if (block.Elements.Length + newChars.Length < MaxBlockSize)
            {
                InsertIntoBlock(blockId, position - block.BlockPosition, newChars);
                return;
            }

            SplitBlock(blockId, position - block.BlockPosition);

            foreach (var batch in newChars.Batch(MaxBlockSize))
            {
                blockId++;
                InsertEmptyBlock(blockId);
                InsertIntoBlock(blockId, 0, batch);
            }
        }

        private void InsertIntoBlock(int blockId, int position, Char[] newChars)
        {
            var block = blocks[blockId];
            var visiblePosition = block.VisibleElements.Search(new CharElement()
                                                               {
                                                                   Position = position
                                                               }, elementPositionComparer);

            for (var i = position; i < block.Elements.Length; i++)
            {
                var nc = block.Elements[i];
                nc.Position += newChars.Length;
                nc.VisiblePosition += newChars.Length;
            }
            for (var i = blockId + 1; i < blocks.Count; i++)
            {
                var nb = blocks[i];
                nb.BlockPosition += newChars.Length;
                nb.BlockVisiblePosition += newChars.Length;
            }
            var charItems = new CharElement[newChars.Length];

            for (var i = 0; i < newChars.Length; i++)
            {
                var nc = new CharElement
                         {
                             Char = newChars[i],
                             Position = position + i,
                             VisiblePosition = visiblePosition + i,
                             Block = block
                         };

                idToChar[nc.Char.Id] = nc;
                charItems[i] = nc;
            }
            block.VisibleElements = block.VisibleElements.Take(visiblePosition)
                                         .Concat(charItems)
                                         .Concat(block.VisibleElements.Skip(visiblePosition))
                                         .FastToArray(block.VisibleElements.Length + charItems.Length);
            block.Elements = block.Elements.Take(position)
                                  .Concat(charItems)
                                  .Concat(block.Elements.Skip(position))
                                  .FastToArray(block.Elements.Length + charItems.Length);
        }

        private void InsertEmptyBlock(int blockId)
        {
            SplitBlock(blockId, 0);
        }

        private void SplitBlock(int blockId, int position)
        {
            var block = blocks[blockId];
            var newBlock = new CharBlock
                           {
                               Id = blockId + 1,
                               BlockPosition = block.BlockPosition + position,
                               VisibleElements = block.VisibleElements.Where(t => t.Position >= position).ToArray(),
                               Elements = block.Elements.Skip(position).ToArray(),
                           };
            block.Elements = block.Elements.Take(position).ToArray();
            block.VisibleElements = block.VisibleElements.Where(t => t.Position < position).ToArray();
            blocks.Insert(blockId + 1, newBlock);
            for (var i = blockId + 2; i < blocks.Count; i++)
                blocks[i].Id++;

            position = 0;
            foreach (var element in newBlock.Elements)
            {
                element.Block = newBlock;
                element.Position = position++;
            }
            newBlock.BlockVisiblePosition = block.BlockVisiblePosition + block.VisibleElements.Length;
            position = 0;
            foreach (var element in newBlock.VisibleElements)
                element.VisiblePosition = position++;
        }

        public void Delete(Char[] toDeleteChars)
        {
            Length -= toDeleteChars.Length;

            var shift = toDeleteChars.Length;
            var lastBlock = blocks.Count - 1;
            int lastPosition = blocks.Last().VisibleElements.Length;

            CharBlock prevBlock = null;
            for (var j = toDeleteChars.Length - 1; j >= 0; j--)
            {
                var ci = idToChar[toDeleteChars[j].Id];
                var cb = ci.Block;

                if (lastBlock >= cb.Id + 1)
                {
                    for (var i = cb.Id + 1; i <= lastBlock; i++)
                        blocks[i].BlockVisiblePosition -= shift;
                    lastPosition = cb.Elements.Length;
                }

                for (var i = ci.VisiblePosition + 1; i < lastPosition; i++)
                    cb.VisibleElements[i].VisiblePosition -= shift;

                if (cb != prevBlock && prevBlock != null)
                    prevBlock.VisibleElements = prevBlock.VisibleElements.Where(t => t.VisiblePosition >= 0).ToArray();
                prevBlock = cb;

                shift--;
                lastBlock = cb.Id;
                lastPosition = ci.VisiblePosition;
                ci.VisiblePosition = -1;
            }
            if (prevBlock != null)
                prevBlock.VisibleElements = prevBlock.VisibleElements.Where(t => t.VisiblePosition >= 0).ToArray();
        }

        public Char GetByVisiblePosition(int visiblePosition)
        {
            var blockId = blocks.Search(new CharBlock
                                        {
                                            BlockVisiblePosition = visiblePosition
                                        }, blockVisiblePositionComparer);
            while (blocks[blockId].VisibleElements.Length == 0)
                blockId++;
            var block = blocks[blockId];
            return block.VisibleElements[visiblePosition - block.BlockVisiblePosition].Char;
        }

        public bool TryGetPositionById(CharId id, out int position)
        {
            CharElement ci;
            if (idToChar.TryGetValue(id, out ci))
            {
                position = ci.Position + ci.Block.BlockPosition;
                return true;
            }
            position = -1;
            return false;
        }

        public IEnumerable<Char> GetChars()
        {
            foreach (var block in blocks)
            foreach (var element in block.Elements)
                yield return element.Char;
        }

        public string BuildString()
        {
            var sb = new StringBuilder();
            foreach (var block in blocks)
            foreach (var s in block.VisibleElements)
                sb.Append(s.Char.Character);
            return sb.ToString();
        }

        private static readonly IComparer<CharBlock> blockPositionComparer = new BlockPositionComparer();

        private class BlockPositionComparer : IComparer<CharBlock>
        {
            public int Compare(CharBlock x, CharBlock y)
            {
                return x.BlockPosition.CompareTo(y.BlockPosition);
            }
        }

        private static readonly IComparer<CharBlock> blockVisiblePositionComparer = new BlockVisiblePositionComparer();

        private class BlockVisiblePositionComparer : IComparer<CharBlock>
        {
            public int Compare(CharBlock x, CharBlock y)
            {
                return x.BlockVisiblePosition.CompareTo(y.BlockVisiblePosition);
            }
        }

        private static readonly IComparer<CharElement> elementPositionComparer = new ElementPositionComparer();

        private class ElementPositionComparer : IComparer<CharElement>
        {
            public int Compare(CharElement x, CharElement y)
            {
                return x.Position.CompareTo(y.Position);
            }
        }
    }
}