using DecimalMath;
using Domain;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using MongoDB.Bson;
using SimDrone.Controllers;


namespace SimDrone;

public class Drone : DroneRecord
{
    private const decimal DroneStepSizeInKilometers = .0004m;

    private SimDroneController _controller;
    public Drone(DroneRecord record, IDroneToDispatchGateway gateway, SimDroneController controller)
    {
        DroneId = record.DroneId;
        HomeLocation = record.HomeLocation;
        DroneUrl = record.DroneUrl;
        _controller = controller;
        State = DroneState.Ready;
        CurrentLocation = HomeLocation;
        Destination = record.Destination;
        Direction = 0m;
    }

    public override string ToString()
    {
        return this.ToJson();
    }

    private async Task<UpdateDroneStatusResponse?> PatchDroneStatus()
    {
        var response = await _controller.UpdateDroneStatus(this);
        return response;
    }

    private void UpdateLocation(GeoLocation location)
    {
        CurrentLocation = location;
        PatchDroneStatus();
    }

    private async Task<UpdateDroneStatusResponse?> UpdateStatus(DroneState state)
    {
        if (State == DroneState.Delivering && state == DroneState.Returning)
        {
            Destination = HomeLocation;
            OrderId = null;
        }

        State = state;
        return await PatchDroneStatus();
    }

    private async Task<UpdateDroneStatusResponse?> UpdateOrderId(DroneState state)
    {
        if (State == DroneState.Delivering && state == DroneState.Returning) Destination = HomeLocation;
        State = state;
        return await PatchDroneStatus();
    }
    
    public async Task TravelTo(GeoLocation endingLocation)
    {
        Destination = endingLocation;
        Console.WriteLine($"Starting at {CurrentLocation.Latitude}");
        var buffer = new GeoLocation[5];
        buffer[0] = CurrentLocation;
        for (var i = 0; !CurrentLocation.Equals(endingLocation); i++)
        {
            Console.WriteLine($"{GeoCalculations.HaversineInMeters(CurrentLocation, endingLocation)} meters away");
            Direction = GeoCalculations.Bearing(CurrentLocation.Latitude, CurrentLocation.Longitude, endingLocation.Latitude,
                endingLocation.Longitude);
            Console.WriteLine($"bearing between = {Direction}");
            buffer[i % 5] = CurrentLocation = GeoCalculations.GetNextLocation(CurrentLocation, Direction, DroneStepSizeInKilometers);
            Console.WriteLine($"{CurrentLocation}");
            if (i % 100 != 0) continue;
            UpdateLocation(CurrentLocation);
            Thread.Sleep(500);
        }
    }
    public async Task<AssignDeliveryResponse> DeliverOrder(AssignDeliveryRequest request)
    {
        OrderId = request.OrderId;
        await UpdateStatus(DroneState.Delivering);
        await TravelTo(request.OrderLocation);
        _controller.CompleteDelivery(
            new CompleteOrderRequest
            {
                DroneId = DroneId,
                OrderId = OrderId,
                Time = DateTime.Now
            });
        Console.WriteLine("Done with delivery, returning home.");
        await UpdateStatus(DroneState.Returning);
        await TravelTo(HomeLocation);
        await UpdateStatus(DroneState.Ready);
        OrderId = request.OrderId;
        return new AssignDeliveryResponse
        {
            DroneId = DroneId,
            OrderId = OrderId,
            Success = true
        };
    }
    public void ChangeController(SimDroneController sim)
    {
        _controller = sim;
    }
}