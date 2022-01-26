using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class EnqueueOrderRequest : BaseDto

{
    [JsonPropertyName("OrderId")] public string OrderId { get; set; }

    [JsonPropertyName("OrderLocation")] public GeoLocation OrderLocation { get; set; }

    public void Deconstruct(out object drone, out object order)
    {
        (drone, order) = (OrderId, OrderLocation);
    }
}