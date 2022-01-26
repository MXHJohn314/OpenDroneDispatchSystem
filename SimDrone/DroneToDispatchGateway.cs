using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;

namespace SimDrone;

public class DroneToDispatchGateway : BaseGateway<SimDroneController>, IDroneToDispatchGateway
{
    public DroneToDispatchGateway(string dispatchUrl)
    {
        Console.WriteLine($"\nThis drone will talk to Dispatch at {dispatchUrl}");
        EndPoint = $"{dispatchUrl}/Dispatch";
    }

    public string EndPoint { get; set; }

    public string GetEndPoint()
    {
        return EndPoint;
    }
    
    public void ChangeHandler(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        new HttpClient(handler);
    }

    public async Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneUpdate request)
    {
        Console.WriteLine($"Drone is updating status to url {EndPoint}");
        return await SendMessagePost<DroneUpdate, UpdateDroneStatusResponse>
            ($"{EndPoint}/UpdateDroneStatus", request);
    }

    public async Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request)
    {
        return await SendMessagePost<CompleteOrderRequest, CompleteOrderResponse>(
            $"{EndPoint}/CompleteOrder", request);
    }

    public async Task<bool> Revive(DroneRecord record)
    {
        return await SendMessagePost<DroneRecord, bool>(
            $"{EndPoint}/Revive", record);
    }
}