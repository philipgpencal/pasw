using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PASW.Domain.Interface.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetById(long id);
        Task Insert(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<T> Find(Expression<Func<T, bool>> predicate);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
