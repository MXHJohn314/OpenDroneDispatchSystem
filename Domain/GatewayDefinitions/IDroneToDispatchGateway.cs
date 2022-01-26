using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;

namespace Domain.GatewayDefinitions;

public interface IDroneToDispatchGateway
{
    public string GetEndPoint();
    public void ChangeHandler(HttpMessageHandler handler);

    public Task<UpdateDroneStatusResponse?> UpdateDroneStatus(DroneUpdate request);

    public Task<CompleteOrderResponse> CompleteDelivery(CompleteOrderRequest request);

    public Task<bool> Revive(DroneRecord record);
}