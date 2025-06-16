using TrtApiService.Data;
using TrtApiService.Models;
using TrtApiService.Repositories;

namespace TrtApiService.Implementation.Repositories.EfCore
{
    public class EfCoreResultRepository : EfCoreRepository<Result>, IResultRepository
    {
        public EfCoreResultRepository(TrtDbContext context) : base(context) { }
    }
}
