using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace SimDrone.Controllers;

[ApiController]
[Route("[controller]")]
public class SimDroneController : ControllerBase
{
    private static Drone _drone;
    private static IDroneToDispatchGateway _gateway;
    private bool IsInitiatead;

    [HttpPost("InitDrone")]
    public async Task<InitDroneResponse> InitDrone(
        InitDroneRequest initDroneRequest)
    {
        var responseString = IsInitiatead
            ? "This drone is already initialized"
            : "This drone is ready to be initialized to a fleet.";
        Console.WriteLine($"SimDroneController.InitDrone -> {initDroneRequest}\tresponse -> {responseString}");
        return new InitDroneResponse
        {
            DroneId = initDroneRequest.DroneId,
            Okay = !IsInitiatead
        };
    }

    [HttpPost("RejoinFleet")]
    public async Task RejoinFleet(RecoveryRequest request)
    {
        Console.WriteLine($"SimDroneController.RejoinFleet({request.Record})");
        await JoinFleet(request.Record);
        await _drone.TravelTo(request.Record.HomeLocation);
    }
    
    
    [HttpPost("HealthCheck")]
    public async Task<DroneRecord> HealthCheck(BaseDto s)
    {
        return new DroneRecord
        {
            DispatchUrl = _drone.DispatchUrl,
            CurrentLocation = _drone.CurrentLocation,
            Destination = _drone.Destination,
            DroneId = _drone.DroneId,
            DroneUrl = _drone.DroneUrl,
            HomeLocation = _drone.HomeLocation,
            OrderId = _drone.OrderId,
            State = _drone.State
        };
    }

    [NonAction]
    public async Task<bool> Revive(DroneRecord record)
    {
        Console.WriteLine($"\nSimDroneController.Revive...");
        if (_gateway == null || string.IsNullOrEmpty(_gateway.GetEndPoint()))
        {
            _gateway = new DroneToDispatchGateway(record.DispatchUrl);
        }
        return await _gateway.Revive(record);
    }
        
    [HttpPost("AssignFleet")]
    public async Task<AssignFleetResponse> AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        Console.WriteLine($"AssignFleet -------------> {assignFleetRequest.DispatchUrl}");
        var droneRecord = new DroneRecord
        {
            CurrentLocation = assignFleetRequest.HomeLocation,
            Destination = assignFleetRequest.HomeLocation,
            DispatchUrl = assignFleetRequest.DispatchUrl,
            DroneUrl = assignFleetRequest.DroneUrl,
            HomeLocation = assignFleetRequest.HomeLocation,
            DroneId = assignFleetRequest.DroneId,
            OrderId = ""
        };
        await JoinFleet(droneRecord);
        Console.WriteLine($"\nSimDrone successfully initialized.\nDrone -->{_drone}");
        IsInitiatead = true;
        //todo write current drone record to file
        await PersistRecord(droneRecord);
        return new AssignFleetResponse
        {
            FirstState = DroneState.Ready,
            DroneId = assignFleetRequest.DroneId,
            IsInitializedAndAssigned = true
        };
    }

    [NonAction]
    private Task JoinFleet(DroneRecord record)
    {
        _drone = new Drone(record, _gateway, this);
        _gateway = new DroneToDispatchGateway(record.DispatchUrl);
        return Task.CompletedTask;
    }

    [NonAction]
    private async Task PersistRecord(DroneRecord droneRecord)
    {
        _drone.DispatchUrl ??= droneRecord.DispatchUrl;
        _drone.DispatchUrl = droneRecord.DispatchUrl;
        Console.WriteLine("\nSaving drone state...");

        FileStream file;
        if (!System.IO.File.Exists(DroneRecord.File()))
        {
            file = System.IO.File.Create(DroneRecord.File());
        }
        else
        {
            file = System.IO.File.Open(DroneRecord.File(), FileMode.Open);
        }
        await using var writer = new StreamWriter(file);
        await writer.WriteAsync(_drone.ToJson());
        writer.Close();
        file.Close();
    }


    [HttpPost("AssignDelivery")]
    public async Task<AssignDeliveryResponse> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        return await _drone.DeliverOrder(assignDeliveryRequest);
    }


    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneRecord updateDroneStatusRequest)
    {
        PersistRecord(updateDroneStatusRequest);
        return await _gateway.UpdateDroneStatus(updateDroneStatusRequest.Update());
    }

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await _gateway.CompleteDelivery(request);
    }
    public void ChangeGateway(IDroneToDispatchGateway mockGatewayObject)
    {
        _gateway = mockGatewayObject;
    }
    public void ChangeDrone(Drone newDrone)
    {
        _drone = newDrone;
    }
}