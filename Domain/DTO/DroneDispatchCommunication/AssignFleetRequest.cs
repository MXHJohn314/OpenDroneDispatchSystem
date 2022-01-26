using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignFleetRequest : BaseDto
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }

    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }

    [JsonPropertyName("DispatchUrl")]
    public string DispatchUrl { get; set; }
    
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
    
    [JsonPropertyName("BadgeNumber")]
    public int BadgeNumber { get; set; }
}