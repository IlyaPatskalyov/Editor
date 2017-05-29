using System.Collections.Generic;

namespace Editor.Model
{
    public interface IEditorString
    {
        Operation[] GenerateOperations();
        Operation[] GenerateOperations(string newValue);
        void ApplyOperations(IEnumerable<Operation> operations);
    }
}