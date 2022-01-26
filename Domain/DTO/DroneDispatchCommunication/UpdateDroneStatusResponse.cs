using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class UpdateDroneStatusResponse : BaseDto
{
    [JsonPropertyName("DroneId")] public string DroneId { get; set; }

    [JsonPropertyName("IsCompletedSuccessfully")]
    public bool IsCompletedSuccessfully { get; set; }
}