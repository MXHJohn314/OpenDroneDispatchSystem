using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryResponse : BaseDto
{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }

    [JsonPropertyName("IsCancelled")] public bool IsCancelled { get; set; }
}