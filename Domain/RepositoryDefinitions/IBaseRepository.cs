using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public interface IBaseRepository<DomainEntity, Update>
{
    /// <summary>
    ///     Creates a new domain entity in the repository
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>The domain entity, as it was created</returns>
    public Task CreateAsync(DomainEntity entity);

    /// <summary>
    ///     Generates a list of all domain entities in the collection.
    /// </summary>
    /// <returns>The the list of domain entities.</returns>
    public Task<List<DomainEntity>> GetAllAsync();


    /// <summary>
    ///     Retrieves a domain entity based on id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The matching entity, or null if it does not exist</returns>
    public Task<DomainEntity> GetByIdAsync(string id);

    /// <summary>
    ///     Deletes the domain entity with the provided id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if the operation succeeded, or false if it failed (including if the entity doesn't exist)</returns>
    public Task<bool> RemoveAsync(string id);

    /// <summary>
    ///     Updates an existing entity in the repository
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>The entity, as it was updated</returns>
    Task<UpdateResult> UpdateAsync(Update update);
}