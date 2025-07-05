using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;
using TrtShared.DTO;

namespace TrtApiService.Implementation.Repositories
{
    public class BranchRepository : Repository<Branch>
    {
        public BranchRepository(TrtDbContext context) : base(context) { }

        /// <summary>
        /// CREATE by branch name
        /// </summary>
        /// <param name="branchName">Branch name to create</param>
        /// <returns>Created Branch</returns>
        public async Task<Branch> CreateAsync(string branchName)
        {
            var branch = new Branch { Name = branchName };
            await _context.Branches.AddAsync(branch);
            return branch;
        }

        /// <summary>
        /// READ OR CREATE
        /// </summary>
        /// <param name="entity">Branch entity</param>
        /// <returns>Existing or created Branch</returns>
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

        /// <summary>
        /// READ OR CREATE by branch name
        /// </summary>
        /// <param name="branchName">Branch name to create</param>
        /// <returns>Existing or created Branch</returns>
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

        /// <summary>
        /// READ. Checks if the branch with specified name already exists
        /// </summary>
        /// <param name="name">Branch name to check</param>
        /// <returns>
        /// True - branch exists
        /// False - no such branch found
        ///</returns>
        public async Task<bool> IsExistsAsync(string name)
        {
            return await _context.Branches.AnyAsync(b => b.Name == name);
        }

        /// <summary>
        /// READ. Checks if the branch with specified id already exists
        /// </summary>
        /// <param name="id">Branch id to check</param>
        /// <returns>
        /// True - branch exists
        /// False - no such branch found
        ///</returns>
        public async Task<bool> IsExistsAsync(int id)
        {
            return await _context.Branches.AnyAsync(b => b.Id == id);
        }

        /// <summary>
        /// UPDATE. Changes name of the current branch
        /// </summary>
        /// <param name="branch">Branch to update</param>
        /// <param name="newName">Name to save for branch</param>
        public void UpdateName(Branch branch, string newName)
        {
            branch.Name = newName;
        }
    }
}
