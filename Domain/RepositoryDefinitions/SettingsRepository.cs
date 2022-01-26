using System;
using System.Threading.Tasks;
using DatabaseAccess;
using Domain.DTO;
using Domain.Entities;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public class SettingsRepository
{
    private IMongoCollection<DispatchSettings> _collection;

    public SettingsRepository(RepositorySettings settings)
    {
        var mongoClient = new MongoClient(
            settings.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.DatabaseName);

        _collection = mongoDatabase.GetCollection<DispatchSettings>(settings.CollectionName);
    }

    public async Task<GeoLocation> GetHomeLocation() => (await _collection.FindAsync(x => true)).First().homeLocation;
}

public class DispatchSettings
{
    public GeoLocation homeLocation { get; set; }
}