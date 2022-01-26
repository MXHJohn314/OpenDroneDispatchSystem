using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseAccess;

public class OrderRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _collection;

    public
        OrderRepository(ODDSSettings settings) //: Domain.InterfaceDefinitions.Repositories
    {
        var mongoClient = new MongoClient(
            settings.CONNECTION_STRING);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.DATABASE_NAME);

        _collection = mongoDatabase.GetCollection<Order>(
            settings.ORDERS_COLLECTION_NAME);
        Console.WriteLine($"this should be 'Orders'>>>{settings.ORDERS_COLLECTION_NAME}<<<");
    }

    public OrderRepository(IMongoCollection<Order> getCollection)
    {
        _collection = getCollection;
    }

    public async Task CreateAsync(Order newOrder)
    {
        Console.WriteLine($"creating:\n{newOrder.ToJson()}");
        await _collection.InsertOneAsync(newOrder);
    }

    public async Task<Order> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.OrderId == id).FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveAsync(string id)
    {
        return (await _collection.DeleteOneAsync(x => x.DroneId == id)).IsAcknowledged;
    }


    public async Task<UpdateResult> UpdateAsync(OrderUpdate update)
    {
        Console.WriteLine($"OrderRepository.UpdateAsync() -> {update.ToJson()}\n(state = {update.State})");
        var filter = Builders<Order>.Filter
            .Eq(o => o.OrderId, update.OrderId);
        var definition = Builders<Order>.Update
            .Set(o => o.DroneId, update.DroneId)
            .Set(o => o.State, update.State)
            .Set(o => o.TimeDelivered, update.TimeDelivered)
            .Set(o => o.HasBeenDelivered, update.HasBeenDelivered);
        return await _collection.UpdateOneAsync(filter, definition, new UpdateOptions {IsUpsert = false});
    }

    public Task<bool>
        Update(Order order)
    {
        var result = _collection.ReplaceOneAsync(
            new BsonDocument("_id", order.DroneId),
            options: new ReplaceOptions {IsUpsert = true},
            replacement: order);
        result.Wait();
        return Task.FromResult(result.IsCompletedSuccessfully);
    }

    public Task UpdateAsync(string id, OrderUpdate update)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<T>(string id, T update)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Order>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

}