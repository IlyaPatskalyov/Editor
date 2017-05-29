using System;
using System.Data.Entity;
using System.Linq;

namespace Editor.Storage.EF
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly IDbContext db;
        private readonly DbSet<Document> set;

        public DocumentsRepository(IDbContext db)
        {
            this.db = db;
            set = this.db.Set<Document>();
        }

        public Document Create(Guid userId)
        {
            return DoTransaction(() =>
                                 {
                                     var document = new Document
                                                    {
                                                        Id = Guid.NewGuid().ToString(),
                                                        UserId = userId.ToString(),
                                                        Created = DateTime.UtcNow,
                                                        Modified = DateTime.UtcNow
                                                    };
                                     set.Add(document);
                                     db.SaveChanges();
                                     return document;
                                 });
        }


        public Document Get(Guid id)
        {
            return set.FirstOrDefault(p => p.Id == id.ToString());
        }

        public Document[] GetByUserId(Guid userId)
        {
            return set.Where(x => x.UserId == userId.ToString()).ToArray();
        }

        public void Delete(Guid id, Guid userId)
        {
            DoTransaction(() =>
                          {
                              var document = set.FirstOrDefault(p => p.Id == id.ToString() && p.UserId == userId.ToString());
                              if (document != null)
                              {
                                  set.Remove(document);
                                  db.SaveChanges();
                              }
                              return document;
                          });
        }

        public void UpdateContent(Guid id, string content)
        {
            DoTransaction(() =>
                          {
                              var document = set.FirstOrDefault(p => p.Id == id.ToString());
                              if (document != null)
                              {
                                  document.Content = content;
                                  document.Modified = DateTime.UtcNow;
                                  db.SaveChanges();
                              }
                              return document;
                          });
        }

        private T DoTransaction<T>(Func<T> action)
        {
            T result;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    result = action();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }
    }
}