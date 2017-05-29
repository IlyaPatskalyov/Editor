using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using SQLite.CodeFirst;

namespace Editor.Storage.EF.SQLite
{
    [DbConfigurationType(typeof(SqLiteDbConfiguration))]
    public class SQLiteDbContext : DbContext, IDbContext
    {
        private readonly IEntityConfigurator[] entityConfigurators;

        public SQLiteDbContext(IDbSettings settings, IEntityConfigurator[] entityConfigurators)
            : base(new SQLiteConnection()
                   {
                       ConnectionString = settings.ConnectionString
                   }, true)
        {
            this.entityConfigurators = entityConfigurators;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new SqliteDropCreateDatabaseWhenModelChanges<SQLiteDbContext>(modelBuilder));

            foreach (var entityConfigurator
                in entityConfigurators)
                entityConfigurator.Configure(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}