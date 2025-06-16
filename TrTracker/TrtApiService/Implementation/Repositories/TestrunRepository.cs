using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Implementation.Repositories
{
    public class TestrunRepository : Repository<Testrun>
    {
        public TestrunRepository(TrtDbContext context) : base(context) { }
    }
}
