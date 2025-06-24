using Microsoft.EntityFrameworkCore;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Implementation.CrudService
{
    public class CrudBranchService : ICrudBranchService
    {
        private readonly TrtDbContext _context;
        private readonly ILogger<CrudBranchService> _logger;

        public CrudBranchService(TrtDbContext context, ILogger<CrudBranchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CreateBranchAsync(Branch branch)
        {
            // Validate BRANCH

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return branch.Id;
        }

        public async Task<Branch?> GetBranchAsync(int id)
        {
            // Validate ID

            var branch = await _context.Branches.FindAsync(id);

            if (branch == null)
            {
                _logger.LogWarning("Branch with id {id} was not found", id);
                return null;
            }

            return branch;
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<bool> RenameBranchAsync(int id, string strNewName)
        {
            if (string.IsNullOrWhiteSpace(strNewName))
            {
                _logger.LogWarning("New name for branch is empty");
                return false;
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                _logger.LogWarning("Branch with id {id} was not found", id);
                return false;
            }

            var branchExists = await _context.Branches.AnyAsync(b => b.Name == strNewName);
            if (branchExists)
            {
                _logger.LogWarning("Branch with name {BranchName} already exists", strNewName);
                return false;
            }

            branch.Name = strNewName;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
