using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Dispatch.Controllers;

[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    private IDispatchToSimDroneGateway _dispatchToSimDroneGateway;

    private readonly IFleetRepository _fleet;
    private readonly IOrdersRepository _orders;
    private readonly GeoLocation _homeLocation;
    private readonly string _dispatchUrl;
    private bool _isInitiatingDrone;

    public DispatchController(IODDSSettings settings)
    {
        _fleet = settings.GetFleetCollection();
        _orders = settings.GetOrdersCollection();
        _homeLocation = settings.GetHomeLocation();
        _dispatchUrl = settings.GetDispatchUrl();
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(_fleet);
    }

    [HttpPost("Recover")]
    public async Task<bool> Revive(DroneRecord record)
    {
        var unsuccessful = true;
        Console.WriteLine("\n\nGot a request to revive a drone.");
        var possibleDrones = (await _fleet.GetAllAsync()).Where(x => x.DroneId.Equals(record.DroneId)).ToList();
        Console.WriteLine($"Possible drones are: [{string.Join(",", possibleDrones)}]");
        if (!possibleDrones.Any()) {
            Console.WriteLine($"All my drones are active right now.  Super suspicious..\ndrone -> {record.ToJson()}");
        } 
        else if (possibleDrones.Count > 1)
        {
            Console.WriteLine("More than one drone is defined with that ID. Super ambiguous and suspicious...");
        }
        else
        {
            Console.WriteLine($"\nDrone matched {record.DroneId}. Reviving drone.");
            record.State = DroneState.Ready;
            await _fleet.UpdateAsync(record.Update());
            unsuccessful = false;
        }

        return unsuccessful;
    }

    [HttpPost("AssignmentCheck")]
    public async Task<BaseDto> AssignmentCheck(BaseDto p)
    {
        await GetNewDrones();
        if (_isInitiatingDrone) return p;
        var orders = (from o in await GetUnfulfilledOrders() where string.IsNullOrEmpty(o.DroneId) select o).ToList();
        var availableDrones =
            (from d in await GetAvailableDrones() where string.IsNullOrEmpty(d.OrderId) select d).ToList();
        var unfulfilledOrders = await GetUnfulfilledOrders();
        Console.WriteLine($"Drones available: [{string.Join(", ", availableDrones.Select(x => x.DroneUrl))}]");
        Console.WriteLine($"Orders waiting: [{string.Join(", ", unfulfilledOrders.Select(x => x.OrderId))}]");
        foreach (var (drone, order) in availableDrones.Zip(unfulfilledOrders))
        {
            var assignment = new AssignDeliveryRequest
                {DroneId = drone.DroneId, OrderId = order.OrderId, OrderLocation = order.DeliveryLocation};
            
            /*do not await!!!*/
            InitiateDelivery(assignment);
        }
        return p;
    }

    [NonAction]
    private async Task GetNewDrones()
    {
        foreach (var drone in (await _fleet.GetAllAsync()).Where(d => d.State == DroneState.Unititialized))
        {
            await AddDrone(drone);
        }
    }

    [NonAction]
    public async Task AddDrone(DroneRecord drone)
    {
        _isInitiatingDrone = true;
        drone.DispatchUrl = _dispatchUrl;
        drone.HomeLocation = _homeLocation;
        Console.WriteLine($"DISPATCH_URL = {_dispatchUrl}");
        var initDroneRequest = new InitDroneRequest
        {
            DroneId = drone.DroneId,
            DroneUrl = drone.DroneUrl
        };

        var initDroneResponse = _dispatchToSimDroneGateway
            .InitDrone(initDroneRequest).Result;
        Console.WriteLine(
            $"\n\n\n\nResponse from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneId:{initDroneResponse.DroneId},Okay:{initDroneResponse.Okay}}}\n\n\n\n");
        if (!initDroneResponse.Okay)
            return;

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = drone.DroneId,
            DroneUrl = drone.DroneUrl,
            DispatchUrl = _dispatchUrl,
            HomeLocation = drone.HomeLocation,
            BadgeNumber = drone.BadgeNumber
        };

        Console.WriteLine($"\n\n\n\nProceeding with _dispatchToSimDroneGateway.AssignFleet({assignFleetRequest.ToJson()}\n\n\n\n");
        var assignFleetResponse = _dispatchToSimDroneGateway.AssignFleet(assignFleetRequest).Result;
        var responseString = assignFleetResponse != null
            ? assignFleetResponse.IsInitializedAndAssigned.ToString()
            : "null";
        Console.WriteLine($"\n\n\n\n_dispatchToSimDroneGateway.AssignFleet - response -> {responseString}\n\n\n\n");

        if (assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {drone.DroneId} was not initiated.");
        }

        Console.WriteLine($"\n\n\n\nsuccess! Saving new drone {drone.DroneId} to repository.\n\n\n\n");
        drone.State = DroneState.Ready;
        drone.OrderId = "";
        drone.DispatchUrl = _dispatchUrl;
        await _fleet.UpdateAsync(drone.Update());

        Console.WriteLine($"\n\n\n\nabout to YEET this drone record:\n{drone.ToJson()}");
    }

    [HttpPost("CompleteOrder")]
    public async Task<CompleteOrderResponse> CompleteOrder(CompleteOrderRequest request)
    {
        var order = new Order
        {
            OrderId = request.OrderId,
            State = OrderState.Delivered,
            TimeDelivered = request.Time,
            DroneId = request.DroneId
        };
        Console.WriteLine(
            $"order {order.OrderId} has been delivered? {order.HasBeenDelivered} @ time {order.TimeDelivered}");
        var result = await _orders.UpdateAsync(order.Update());
        return new CompleteOrderResponse
        {
            IsAcknowledged = result.IsAcknowledged
        };
    }

    [HttpPost("AssignDelivery")]
    private async Task<AssignDeliveryResponse> InitiateDelivery(AssignDeliveryRequest request)
    {
        if (request == null)
        {
            return new AssignDeliveryResponse {DroneId = request.DroneId, Success = false};
        }

        // Console.WriteLine("!!!!!!" + request.ToJson());
        // Console.WriteLine($"In DispatchController.AssignDeliveryResponse, order :{request.ToJson()}");

        var order = await _orders.GetByIdAsync(request.OrderId);
        var drone = await _fleet.GetByIdAsync(request.DroneId);

        var s1 = $"drone.OrderId {drone.OrderId} -> ";
        var s2 = $"order.DroneId {order.DroneId} -> ";
        order.DroneId = drone.DroneId;
        drone.OrderId = order.OrderId;
        s1 += $"{drone.OrderId}";
        s2 += $"{order.DroneId}";
        
        Console.WriteLine($"{s1}\n{s2}");
        order.State = OrderState.Assigned;
        drone.State = DroneState.Assigned;

        await _fleet.UpdateAsync(drone.Update());
        await _orders.UpdateAsync(order.Update());

        return await _dispatchToSimDroneGateway.AssignDelivery(request);
    }


    [NonAction]
    private async Task<IEnumerable<Order>> GetUnfulfilledOrders()
    {
        var orders = await _orders.GetAllAsync();
        Console.WriteLine($"All the orders: {orders.Count}");
        var unassignedOrders = orders.Where(o =>
            !o.HasBeenDelivered && o is {State: OrderState.Waiting} && string.IsNullOrEmpty(o.DroneId));
        return unassignedOrders;
    }

    [NonAction]
    private async Task<IEnumerable<DroneRecord>> GetAvailableDrones()
    {
        if (_isInitiatingDrone)
        {
            Console.WriteLine("Thread lock, cannot read drones at this time.");
            return new List<DroneRecord>();
        }
        try
        {
            Console.WriteLine("DequeueOrders...");
            var drones = from d in await _fleet.GetAllAsync()
                where d != null
                      && d.State == DroneState.Ready
                      && string.IsNullOrEmpty(d.OrderId)
                select d;
            var availabledDrones = new List<DroneRecord>();
            foreach (var drone in drones)
            {
                if (!await _dispatchToSimDroneGateway.HealthCheck(drone.DroneId)) continue;
                Console.WriteLine($"drone at {drone.DroneUrl} is healthy");
                availabledDrones.Add(drone);
            }

            return availabledDrones;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<DroneRecord>();
        }
    }

    [HttpPost("PostInitialStatus")]
    public async Task<UpdateDroneStatusResponse> PostInitialStatus(DroneUpdate initialStatusRequest)
    {
        // Console.WriteLine($"DispatchController.PostInitialStatus -> {initialStatusRequest}");
        return await UpdateDroneStatus(initialStatusRequest);
    }

    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse> UpdateDroneStatus(DroneUpdate droneStatusRequest)
    {
        // Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        var updateResult = await _fleet.UpdateAsync(droneStatusRequest);
        var response = new UpdateDroneStatusResponse
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = updateResult.IsAcknowledged && updateResult.ModifiedCount == 1
        };
        // Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");

        return response;
    }


    [HttpPost("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> GetDroneById(string requestedDroneId)
    {
        // Console.WriteLine($"DispatchController.Get -> {requestedDroneId}");
        var droneRecord = await _fleet.GetByIdAsync(requestedDroneId);

        if (droneRecord is null) return NotFound();

        return droneRecord;
    }
    [NonAction]
    public void ChangeGateway(IDispatchToSimDroneGateway mockedGate)
    {
        _dispatchToSimDroneGateway = mockedGate;
    }
}