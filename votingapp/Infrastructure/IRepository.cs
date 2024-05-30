
using System.Data;
using System.Linq.Expressions;

namespace votingapp.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        Task<T> AddAsync(T entity);
        Task<List<T>> GetAllAsyncList();
        T Find(Expression<Func<T, bool>> match);
        T Update(T updated, int key);
    }
}