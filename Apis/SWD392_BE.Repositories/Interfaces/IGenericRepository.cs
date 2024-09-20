using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWD392_BE.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        ICollection<T> Get();
        ICollection<T> GetList(Expression<Func<T, bool>> expression);
        T Get(Expression<Func<T, bool>> expression);
        T Add(T entity);
        void AddRange(ICollection<T> entities);
        void Update(T entity);
        void Delete(string id);
        void Remove(T enity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        void ClearTrackers();
        int SaveChanges();
        Task SaveChangesAsync();
        void Dispose();
    }
}
