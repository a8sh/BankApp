using BankApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankApp.Repository
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        private readonly Context _context;
        private readonly DbSet<T> _table;
        private IDbContextTransaction _transaction;

        public EntityRepository(Context context)
        {
            _context = context;
            _table = _context.Set<T>();
        }
        public bool Add(T entity)
        {
            _table.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }

        public bool Delete(Guid id)
        {
            var entity = GetById(id);
            if(entity == null)
            {
                return false;
            }
            var IsActiveProperty = entity.GetType().GetProperty("IsActive");
            if(IsActiveProperty != null)
            {
                IsActiveProperty.SetValue(entity, false);
                _table.Update(entity);
            }
            else
            {
                _table.Remove(entity);
            }
            _context.SaveChanges();
            return true;
        }

        public void Detach(T entity)
        {
            var entry = _context.Entry(entity);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
        }

        public IQueryable<T> GetAll()
        {
            return _table.AsQueryable();
        }

        public T GetById(Guid id)
        {
            var entity = _table.Find(id);
            if (entity == null)
            {
                return entity;
            }
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public T Update(T entity)
        {
            _table.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
