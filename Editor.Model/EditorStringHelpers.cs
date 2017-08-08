using System;

namespace Editor.Model
{
    public static class EditorStringHelpers
    {
        public static bool AreEqualClientId(string operation, Guid clientId)
        {
            return OperationSerializer.Deserialize(operation).Char.Id.AreEqualClientId(clientId);
        }
    }
}