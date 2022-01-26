using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneRequest : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("DroneUrl")] public string DroneUrl { get; set; }
}