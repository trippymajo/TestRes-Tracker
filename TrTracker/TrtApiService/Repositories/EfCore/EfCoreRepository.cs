using TrtApiService.Data;

namespace TrtApiService.Repositories.EfCore
{
    public class EfCoreRepository<T> : IRepository<T> where T : class
    {
        protected readonly TrtDbContext _context;

        public EfCoreRepository(TrtDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task CreateAsync(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
        }
    }
}
