using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public interface IOrdersRepository : IBaseRepository<Order, OrderUpdate>
{
    public Task<List<Order>> GetAllAsync();
    Task<UpdateResult> UpdateAsync(OrderUpdate update);
}