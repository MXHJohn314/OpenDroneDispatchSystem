using Dispatch.Controllers;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;

namespace Dispatch;

public class DispatchToSimDroneGateway : BaseGateway<DispatchController>, IDispatchToSimDroneGateway
{
    private readonly IFleetRepository _fleet;

    public DispatchToSimDroneGateway(IFleetRepository fleet)
    {
        _fleet = fleet;
    }

    private async Task<string> Endpoint(string currentDroneId)
    {
        Console.WriteLine($"Getting by drone id {currentDroneId}");
        return (await _fleet.GetByIdAsync(currentDroneId)).DroneUrl;
    }

    public async Task<InitDroneResponse?> InitDrone(InitDroneRequest request)
    {
        var url = $"{request.DroneUrl}/SimDrone/InitDrone";
        Console.WriteLine($"\n\n\nurl:{url}\n");


        return await SendMessagePost<InitDroneRequest, InitDroneResponse>(
            url, request);
    }

    public async Task<AssignFleetResponse?> AssignFleet(AssignFleetRequest assignFleetRequest)
    {
        if (assignFleetRequest.DispatchUrl is null or "")
        {
            Console.WriteLine("A Dispatch Ip Address is requred!");
            return new AssignFleetResponse
            {
                DroneId = assignFleetRequest.DroneId,
                IsInitializedAndAssigned = false
            };
        }

        var droneUrl = $"{assignFleetRequest.DroneUrl}/SimDrone/AssignFleet";
        Console.WriteLine($"Sending {assignFleetRequest.DispatchUrl} to the drone @ {droneUrl} so it can talk to us...");
        Console.WriteLine(
            $"\n\n\n\n\n!!!!!!!!!!!assignFleetRequest.DispatchUrl = {assignFleetRequest.DispatchUrl}\n\n\n\n");

        var response = SendMessagePost<AssignFleetRequest, AssignFleetResponse>(
            droneUrl, assignFleetRequest);


        response.Wait();
        return response.Result;
    }

    public async Task<AssignDeliveryResponse?> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest)
    {
        var url = await Endpoint(assignDeliveryRequest.DroneId);
        Console.WriteLine($"\n\nChoosing drone {assignDeliveryRequest.DroneId} url:{url}\n\n");
        return await SendMessagePost<AssignDeliveryRequest, AssignDeliveryResponse>(
            $"{url}/SimDrone/AssignDelivery", assignDeliveryRequest);
    }

    public async Task<bool> HealthCheck(string droneId)
    {
        var url = await Endpoint(droneId);
        DroneRecord droneRecord;
        Console.WriteLine($"DispatchToSimDroneGateway.HealthCheck with url {url}/SimDrone/HealthCheck");
        try
        {
            droneRecord = await SendMessagePost<BaseDto, DroneRecord>($"{url}/SimDrone/HealthCheck", new BaseDto {Message = "HealthCheck"});
            Console.WriteLine("Awaiting for response...");
            if (droneRecord is {State: DroneState.Ready})
                return true;
        }
        catch (Exception e)
        {
            // Todo, find a way to update the drone without all other update params and only ID, set it to DroneState.Dead
            Console.WriteLine($"Setting drone {droneId} offline.");
            await _fleet.SetDroneOffline(droneId);
            Console.WriteLine($"HealthCheck failed for drone at {droneId}. Reason: {e}");
        }
        return false;
    }
    public void ChangeHandle(HttpMessageHandler handle)
    {
        _httpClient = new HttpClient(handle);
    }
}