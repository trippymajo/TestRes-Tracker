using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;
using TrtApiService.Repositories;

namespace TrtApiService.Implementation.Repositories.EfCore
{
    public class EfCoreBranchRepository : EfCoreRepository<Branch>, IBranchRepository
    {
        public EfCoreBranchRepository(TrtDbContext context) : base(context) { }

        public async Task<Branch> CreateAsync(string branchName)
        {
            var branch = new Branch { Name = branchName };
            await _context.Branches.AddAsync(branch);
            return branch;
        }

        public async Task<Branch> GetOrCreateAsync(Branch entity)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Name == entity.Name);

            // If branch name was not found
            if (branch == null)
            {
                await _context.Branches.AddAsync(entity);
                return entity;
            }

            return branch;
        }

        public async Task<Branch> GetOrCreateAsync(string branchName)
        {
            var branch = await _context.Branches
                    .FirstOrDefaultAsync(b => b.Name == branchName);

            // If branch name was not found
            if (branch == null)
            {
                branch = new Branch { Name = branchName };
                await _context.Branches.AddAsync(branch);
            }

            return branch;
        }
    }
}
