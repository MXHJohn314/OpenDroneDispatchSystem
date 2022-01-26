using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderResponse : BaseDto
{
    [JsonPropertyName("IsAcknowledged")] public bool IsAcknowledged { get; set; }
}