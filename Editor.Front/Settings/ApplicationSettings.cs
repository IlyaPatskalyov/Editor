using System.Configuration;

namespace Editor.Front.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string ConnectionString { get; private set; }

        public ApplicationSettings()
        {
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
    }
}