using Microsoft.EntityFrameworkCore;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.Models;
using TrtShared.RetValType;

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

        public async Task<RetVal<int>> CreateBranchAsync(Branch branch)
        {
            // Validate BRANCH

            try
            {
                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errMsg = "Creating new branch failed";
                _logger.LogError(ex, errMsg);
                return RetVal<int>.Fail(ErrorType.ServerError, errMsg);
            }

            return RetVal<int>.Ok(branch.Id);
        }

        public async Task<RetVal> DeleteBranchAsync(int id)
        {
            Branch? branch = null;
            try
            {
                branch = await _context.Branches.FindAsync(id);

                if (branch == null)
                {
                    var errMsg = $"Branch with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                _context.Branches.Remove(branch);
            }
            catch (Exception ex)
            {
                var errMsg = $"Deleting the branche with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }

            return RetVal.Ok();
        }

        public async Task<RetVal<Branch>> GetBranchAsync(int id)
        {
            Branch? branch = null;
            try
            {
                branch = await _context.Branches.FindAsync(id);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting branch with id {id} failed"; ;
                _logger.LogError(ex, errMsg);
                return RetVal<Branch>.Fail(ErrorType.ServerError, errMsg);
            }

            if (branch == null)
            {
                var errMsg = $"Branch with id {id} was not found";
                _logger.LogWarning(errMsg);
                return RetVal<Branch>.Fail(ErrorType.NotFound, errMsg);
            }

            return RetVal<Branch>.Ok(branch);
        }

        public async Task<RetVal<IEnumerable<Branch>>> GetBranchesAsync()
        {
            List<Branch>branchList;

            try
            {
                branchList = await _context.Branches.ToListAsync();
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting all the branches failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Branch>>.Fail(ErrorType.ServerError, errMsg);
            }

            return RetVal<IEnumerable<Branch>>.Ok(branchList);
        }

        public async Task<RetVal> RenameBranchAsync(int id, string strNewName)
        {
            if (string.IsNullOrWhiteSpace(strNewName))
            {
                var errMsg = "New name for branch is empty";
                _logger.LogWarning(errMsg);
                return RetVal.Fail(ErrorType.BadRequest, errMsg);
            }

            try
            {
                var branch = await _context.Branches.FindAsync(id);
                if (branch == null)
                {
                    var errMsg = $"Branch with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                var branchExists = await _context.Branches.AnyAsync(b => b.Name == strNewName);
                if (branchExists)
                {
                    var errMsg = $"Branch with name {strNewName} already exists";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                branch.Name = strNewName;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errMsg = $"Renaming the branch id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Branch>>.Fail(ErrorType.ServerError, errMsg);
            }

            return RetVal.Ok();
        }
    }
}
