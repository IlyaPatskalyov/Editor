using System;
using System.Collections.Generic;

namespace Editor.Model
{
    public class CharId
    {
        public CharId()
        {
        }

        public CharId(Guid clientId, int operationId)
        {
            ClientId = clientId.ToString().Substring(0, 8);
            OperationId = operationId;
        }

        public string ClientId { get; set; }
        public int OperationId { get; set; }

        public sealed class ClientIdOperationIdEqualityComparer : IEqualityComparer<CharId>
        {
            public bool Equals(CharId x, CharId y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return string.Equals(x.ClientId, y.ClientId) && x.OperationId == y.OperationId;
            }

            public int GetHashCode(CharId obj)
            {
                unchecked
                {
                    return ((obj.ClientId != null ? obj.ClientId.GetHashCode() : 0) * 397) ^ obj.OperationId;
                }
            }
        }

        public static ClientIdOperationIdEqualityComparer Comparer { get; } = new ClientIdOperationIdEqualityComparer();
    }
}