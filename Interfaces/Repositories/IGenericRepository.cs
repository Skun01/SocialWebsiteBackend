using System;

namespace SocialWebsite.Interfaces.Repositories;

public interface IGenericRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task UpdateAsync(T entity);
    Task<T> AddAsync(T entity);
    Task DeleteAsync(T entity);
}
