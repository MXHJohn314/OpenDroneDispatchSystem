using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryRequest : BaseDto
{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }
}