using System;
using System.Collections.Generic;

namespace Editor.Model
{
    internal class CharId
    {
        public CharId()
        {
        }

        public CharId(Guid clientId, uint operationId)
        {
            Value = TransformClientId(clientId) << 24 | operationId;
        }

        public CharId(uint value)
        {
            Value = value;
        }

        private static uint TransformClientId(Guid clientId)
        {
            return BitConverter.ToUInt32(clientId.ToByteArray(), 0) & 0xFF;
        }

        public readonly uint Value;

        public static int Compare(CharId newCharId, CharId l)
        {
            return l.Value.CompareTo(newCharId.Value);
        }

        public bool AreEqualClientId(Guid clientId)
        {
            return (Value >> 24) == TransformClientId(clientId);
        }

        public static ClientIdOperationIdEqualityComparer Comparer { get; } = new ClientIdOperationIdEqualityComparer();

        public sealed class ClientIdOperationIdEqualityComparer : IEqualityComparer<CharId>
        {
            public bool Equals(CharId x, CharId y)
            {
                return x.Value == y.Value;
            }

            public int GetHashCode(CharId obj)
            {
                return (int) obj.Value;
            }
        }
    }
}