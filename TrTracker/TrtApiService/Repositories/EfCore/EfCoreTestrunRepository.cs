using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Repositories.EfCore
{
    public class EfCoreTestrunRepository : EfCoreRepository<Testrun> //, ITestrunRepository
    {
        public EfCoreTestrunRepository(TrtDbContext context) : base(context) { }
    }
}
