namespace Aptiverse.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T?> GetOneAsync(long id);
        Task<IEnumerable<T>> GetManyAsync(Dictionary<string, object>? filters = null);
        Task<T> UpdateAsync(long id, T entity);
        Task DeleteAsync(long id);
    }
}
