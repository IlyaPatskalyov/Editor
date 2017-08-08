using System.Configuration;

namespace Editor.Front.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {
        public ApplicationSettings()
        {
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            LogPath = ConfigurationManager.AppSettings["LogPath"];
        }

        public string ConnectionString { get; }
        public string LogPath { get; }
    }
}