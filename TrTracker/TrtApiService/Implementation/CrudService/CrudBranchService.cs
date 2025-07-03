using Microsoft.EntityFrameworkCore;

using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
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

        public async Task<RetVal<int>> CreateBranchAsync(CreateBranchDTO branchDto)
        {
            if (string.IsNullOrWhiteSpace(branchDto.Name))
            {
                var errMsg = "Name is required";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            try
            {
                if (await IsBranchExists(branchDto.Name))
                {
                    var errMsg = $"Branch with name {branchDto.Name} already exists";
                    _logger.LogWarning(errMsg);
                    return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
                }

                var branch = new Branch
                {
                    Name = branchDto.Name,
                };

                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();

                return RetVal<int>.Ok(branch.Id);
            }
            catch (Exception ex)
            {
                var errMsg = "Creating new branch failed";
                _logger.LogError(ex, errMsg);
                return RetVal<int>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> DeleteBranchAsync(int id)
        {
            try
            {
                var branch = await _context.Branches.FindAsync(id);

                if (branch == null)
                {
                    var errMsg = $"Branch with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                _context.Branches.Remove(branch);

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Deleting the branche with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<Branch>> GetBranchAsync(int id)
        {
            try
            {
                var branch = await _context.Branches.FindAsync(id);

                if (branch == null)
                {
                    var errMsg = $"Branch with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal<Branch>.Fail(ErrorType.NotFound, errMsg);
                }

                return RetVal<Branch>.Ok(branch);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting branch with id {id} failed"; ;
                _logger.LogError(ex, errMsg);
                return RetVal<Branch>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<IEnumerable<Branch>>> GetBranchesAsync()
        {
            try
            {
                var branchList = await _context.Branches.ToListAsync();

                return RetVal<IEnumerable<Branch>>.Ok(branchList);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting all the branches failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Branch>>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> UpdateBranchAsync(int id, UpdateBranchDTO branchDto)
        {
            if (string.IsNullOrWhiteSpace(branchDto.Name))
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

                if (await IsBranchExists(branchDto.Name))
                {
                    var errMsg = $"Branch with name {branchDto.Name} already exists";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                branch.Name = branchDto.Name;
                await _context.SaveChangesAsync();

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Renaming the branch id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }

        /// <summary>
        /// Checks if the branch with specified name already exists
        /// </summary>
        /// <param name="branchName">Branch name to check</param>
        /// <returns>
        /// True - branch exists
        /// False - no such branch found
        ///</returns>
        private async Task<bool> IsBranchExists(string branchName)
        {
            return await _context.Branches.AnyAsync(b => b.Name == branchName);
        }
    }
}
