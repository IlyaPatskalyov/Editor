using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;

namespace Editor.Storage.EF
{
    public class DocumentEntityConfigurator : IEntityConfigurator
    {
        public void Configure(DbModelBuilder modelBuilder)
        {
            var ec = modelBuilder.Entity<Document>();
            ec.ToTable("Documents");
            ec.HasKey(p => p.Id);
            ec.Property(p => p.UserId)
              .IsRequired()
              .HasColumnAnnotation(
                  IndexAnnotation.AnnotationName,
                  new IndexAnnotation(new IndexAttribute("IX_UserId", 1)));
            ;
        }
    }
}