using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YK.Application.Common.Abstractions.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T?> GetByIdAsync(long Id);
        Task<T> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllByPredicateAsync(Expression<Func<T, bool>> predicate);
    }
}
