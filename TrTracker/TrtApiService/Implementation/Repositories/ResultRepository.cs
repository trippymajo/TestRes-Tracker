using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Implementation.Repositories
{
    public class ResultRepository : Repository<Result>
    {
        public ResultRepository(TrtDbContext context) : base(context) { }
    }
}
