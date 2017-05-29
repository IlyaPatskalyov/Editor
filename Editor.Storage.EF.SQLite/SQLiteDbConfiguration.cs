using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace Editor.Storage.EF.SQLite
{
    public class SqLiteDbConfiguration : DbConfiguration
    {
        public SqLiteDbConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices) SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}