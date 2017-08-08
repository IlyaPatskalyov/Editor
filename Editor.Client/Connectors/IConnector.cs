namespace Editor.Client.Connectors
{
    public interface IConnector
    {
        void Start();
        void Stop();
        void SendPosition(int pos);
        void SendOperatinos(string[] operations);
    }
}