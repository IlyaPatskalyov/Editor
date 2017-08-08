namespace Editor.Model
{
    internal class Operation
    {
        public Operation(OperationType operationType, Char c)
        {
            OperationType = operationType;
            Char = c;
        }

        public readonly OperationType OperationType;

        public readonly Char Char;
    }
}