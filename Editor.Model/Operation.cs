namespace Editor.Model
{
    public class Operation
    {
        public Operation()
        {
        }

        public Operation(OperationType operationType, Char c)
        {
            OperationType = operationType;
            Char = c;
        }

        public OperationType OperationType { get; set; }

        public Char Char { get; set; }
    }
}