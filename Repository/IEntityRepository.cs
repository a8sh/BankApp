using Microsoft.EntityFrameworkCore.Storage;

namespace BankApp.Repository
{
    public interface IEntityRepository<T>
    {
        public IQueryable<T> GetAll();
        public T GetById(Guid id);
        public bool Add(T entity);
        public T Update(T entity);
        public bool Delete(Guid id);
        public IDbContextTransaction BeginTransaction();
        public void Detach(T entity);
    }
}
