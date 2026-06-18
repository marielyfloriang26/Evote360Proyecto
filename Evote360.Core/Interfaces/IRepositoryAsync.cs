using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evote360.Core.Interfaces
{
    public interface IRepositoryAsync<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
