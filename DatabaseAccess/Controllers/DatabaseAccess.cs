using DatabaseAccess.Services;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DatabaseAccess.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseAccess : ControllerBase
{
    private readonly IFleetRepository _fleet;
    private readonly IOrdersRepository _orders;
    private readonly string _apiKey;
    private readonly string DispatchUrl;
    private readonly GeoLocation HomeLocation;

    public DatabaseAccess(IODDSSettings settings)
    {
        _fleet = settings.GetFleetCollection();
        _orders = settings.GetOrdersCollection();
        _apiKey = settings.GetAPIKey();
        HomeLocation = settings.GetHomeLocation();
        DispatchUrl = settings.GetDispatchUrl();
    }

    [HttpGet("GetFleet")]
    public async Task<List<DroneRecord>> GetFleet()
    {
        //Console.WriteLine("got a request to get the fleet...");
        var fleet =  await _fleet.GetAllAsync();
        //Console.WriteLine("Got back" + string.Join("\n", fleet));
        return fleet;
    }

    [HttpGet("GetOrders")]
    public async Task<List<Order>> GetOrders()
    {
        //Console.WriteLine("got a request to get the orders...");
        var orders = await _orders.GetAllAsync();
        //Console.WriteLine("Got back" + string.Join("\n", orders));
        return orders;
    }

    [HttpPost("CancelOrder")]
    public async Task<CancelDeliveryResponse> CancelOrder(string OrderId)
    {
        if (string.IsNullOrEmpty(OrderId)) new CancelDeliveryResponse();
        await _orders.RemoveAsync(OrderId);
        return new CancelDeliveryResponse();
    }

    [HttpPost("CreateOrder")]
    public async Task<CreateOrderResponse> CreateOrder(Order order)
    {
        order.DeliveryLocation = await LocationParser.Parse(_apiKey, order.DeliveryAddress);
        //Console.WriteLine($"\n\n\n\n\nadding order: {order.ToJson()}\n\n\n\n\n");
        await _orders.CreateAsync(order);
        return new CreateOrderResponse();
    }
    
    [HttpGet("GetHomeLocation")]
    public async Task<GeoLocation> GetHomeLocation()
    {
        //Console.WriteLine($"\n\n\n\nGetting HomeLocation:");
        return HomeLocation;
    }
  
    
    [HttpPost("AddDrone")]
    public async Task AddDrone(BaseDto @base)
    {
        var droneUrl = @base.Message;
        Console.WriteLine($"\n\n\n\n\n\ngot a new DRONE!!!\n@{droneUrl}");
        var drone = new DroneRecord
        {
            BadgeNumber = await GetNextBadgeNumber(),
            DroneId = BaseEntity.GenerateNewId(),
            DroneUrl = droneUrl,
            HomeLocation = HomeLocation,
            CurrentLocation = HomeLocation,
            Destination = HomeLocation,
            State = DroneState.Unititialized,
            OrderId = null,
            DispatchUrl = DispatchUrl
        };
        //Console.WriteLine($"About to YEET this drone record...{drone}\n\n\n\n\n");
        await _fleet.CreateAsync(drone);
    }

    private async Task<int> GetNextBadgeNumber()
    {
        var drones = await _fleet.GetAllAsync();
        if (!drones.Any())
        {
            return 1;
        }
        var numbers = (drones).Select(x => x.BadgeNumber);
        var maxDroneNumber = numbers.Max();
        foreach (var i in Enumerable.Range(1, maxDroneNumber + 1))
        {
            if (!numbers.Contains(i))
            {
                return i;
            }
        }

        return maxDroneNumber + 1;
    }

    [HttpGet("GetDrone")]
    public DroneRecord GetDrone(string id)
    {
        return _fleet.GetByIdAsync(id).Result;
    }


    [HttpGet("GetDispatchUrl")]
    public string GetDispatchUrl()
    {
        return DispatchUrl;
    }

    [HttpGet("GetOrderById")]
    public Order GetOrderById(string id)
    {
        return _orders.GetByIdAsync(id).Result;
    }
}