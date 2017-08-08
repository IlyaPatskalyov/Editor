using Editor.Storage.EF;

namespace Editor.Front.Settings
{
    public interface IApplicationSettings : IDbSettings
    {
        string LogPath { get; }
    }
}