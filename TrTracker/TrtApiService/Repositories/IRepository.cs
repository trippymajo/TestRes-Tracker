namespace TrtApiService.Repositories
{
    /// <summary>
    /// Repository pattern to work with tables in DB.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// CREATE. Creates new item in item table in DB
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>
        /// Reference to just created entity.
        /// [null] - Entity already exists or the problem occured
        /// </returns>
        Task<T> CreateAsync(T entity);

        //Task DeleteBranchAsync(); // DELETE
        //ask RenameBranchAsync( , string newName); // UPDATE
        //Task<Branch> ReadAllBranchNames(); // READ
    }
}
