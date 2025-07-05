using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;

// NOTE: This is a bit overkill, but non the less, if you need ASP.NET,
// this is the place where you can easily change everything. This was done
// in order to keep everything in one place to avoid hard times while refactoring
// when changing ORM or Framework over DB

namespace TrtApiService.Implementation.Repositories
{
    public class Repository<T> where T : class
    {
        protected readonly TrtDbContext _context;

        public Repository(TrtDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// CREATE
        /// </summary>
        /// <param name="entity">Model entity</param>
        /// <returns>Created model entity</returns>
        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// CREATE BATCH
        /// </summary>
        /// <param name="entities">Enumerable with Model entities to create</param>
        public async Task CreateAsync(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
        }

        /// <summary>
        /// READ
        /// </summary>
        /// <returns>List of Model entities</returns>
        public async Task<IEnumerable<T>> GetAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// READ. Find entitiy by id
        /// </summary>
        /// <param name="id">Entity's id</param>
        /// <returns>Model entity</returns>
        public async Task<T?> FindByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// UPDATE
        /// </summary>
        /// <param name="entity">Model entity</param>
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity); 
        }

        /// <summary>
        /// REMOVE
        /// </summary>
        /// <param name="entity">Model entity</param>
        public void Remove(T entity)
        {
            _context.Remove(entity);
        }
    }
}
