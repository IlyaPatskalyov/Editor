using System.Data.Entity;

namespace Editor.Storage.EF
{
    public interface IEntityConfigurator
    {
        void Configure(DbModelBuilder modelBuilder);
    }
}