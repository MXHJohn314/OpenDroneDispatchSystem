using System;
using DatabaseAccess;
using Domain.DTO;
using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Domain.RepositoryDefinitions;

public class ODDSSettings : IODDSSettings
{
    public ODDSSettings(RepositorySettings dbSettings)
    {
        var mongoClient = new MongoClient(
            dbSettings.ConnectionString);

        _mongoDatabase = mongoClient.GetDatabase(
            dbSettings.DatabaseName);

        var query = _mongoDatabase.GetCollection<ODDSSettings>("Settings")
            .Find(x => x._id.Equals(dbSettings.CollectionName)).First();
        DISPATCH_URL = query.DISPATCH_URL;
        DATABASE_NAME = query.DATABASE_NAME;
        FLEET_COLLECTION_NAME = query.FLEET_COLLECTION_NAME;
        ORDERS_COLLECTION_NAME = query.ORDERS_COLLECTION_NAME;
        HOME_LOCATION = query.HOME_LOCATION;
        API_KEY = query.API_KEY;
        CONNECTION_STRING = query.CONNECTION_STRING;
        DATABASE_ACCESS_URL = query.DATABASE_ACCESS_URL;


    }
    public string _id = "settings";
    private IMongoDatabase _mongoDatabase;
    public string DISPATCH_URL { get; set;}
    public string DATABASE_ACCESS_URL { get; set; }
    public string FLEET_COLLECTION_NAME { get; set;}
    public string ORDERS_COLLECTION_NAME { get; set;}
    public GeoLocation HOME_LOCATION { get; set; }
    public string API_KEY { get; set;}
    public string CONNECTION_STRING { get; set;}
    public string DATABASE_NAME { get; set;}

    public IFleetRepository GetFleetCollection()
    {
        Console.WriteLine($"Getting '{FLEET_COLLECTION_NAME}'");
        return new FleetRepository(_mongoDatabase.GetCollection<DroneRecord>(FLEET_COLLECTION_NAME));
    }

    public IOrdersRepository GetOrdersCollection()
    {
        Console.WriteLine($"Getting '{ORDERS_COLLECTION_NAME}'");
        return new OrderRepository(_mongoDatabase.GetCollection<Order>(ORDERS_COLLECTION_NAME));
    }

    public string? GetAPIKey()
    {
        return API_KEY;
    }

    public GeoLocation GetHomeLocation()
    {
        return HOME_LOCATION;
    }

    public string GetDispatchUrl()
    {
        return DISPATCH_URL;
    }
}