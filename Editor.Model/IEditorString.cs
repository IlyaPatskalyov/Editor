using System.Collections.Generic;

namespace Editor.Model
{
    public interface IEditorString
    {
        string[] GenerateOperations(string newValue);
        void ApplyOperations(ICollection<string> ops);
    }
}