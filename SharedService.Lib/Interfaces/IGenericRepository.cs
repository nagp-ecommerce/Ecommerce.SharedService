using System.Linq.Expressions;

namespace SharedService.Lib.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<Response> CreateAsync(T Entity);
        // adding a new entity to the database
        public Task<Response> UpdateAsync(T Entity);
        // Updates an existing entity in database
        public Task<Response> DeleteAsync(int id);
        // Deletes an existing entity in database
        public Task<IEnumerable<T>> GetAllAsync();
        // Retrieves all entities from database
        public Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        // to query data asynchronously based on a condition or filter

        public Task<T> GetByIdAsync(int id);
        // Retrieve by Id
    }
}