using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;

namespace Domain.GatewayDefinitions;

public interface IDispatchToSimDroneGateway
{
    public Task<InitDroneResponse?> InitDrone(InitDroneRequest request);

    public Task<AssignFleetResponse?> AssignFleet(AssignFleetRequest assignFleetRequest);

    public Task<AssignDeliveryResponse?> AssignDelivery(AssignDeliveryRequest assignDeliveryRequest);
    public Task<bool> HealthCheck(string droneId);
}