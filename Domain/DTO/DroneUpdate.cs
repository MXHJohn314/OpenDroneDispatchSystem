using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DTO;

public class DroneUpdate
{
    [BsonElement("DroneId")]
    [JsonProperty("DroneId")]
    public string DroneId { get; set; }
    
    [BsonElement("Direction")]
    [JsonProperty("Direction")]
    public decimal Direction { get; set; }

    [BsonElement("State")]
    [JsonProperty("State")]
    public DroneState State { get; set; }

    [BsonElement("CurrentLoacation")]
    [JsonProperty("CurrentLoacation")]
    public GeoLocation CurrentLocation { get; set; }

    [BsonElement("Destination")]
    [JsonProperty("Destination")]
    public GeoLocation Destination { get; set; }

    [BsonElement("OrderId")]
    [JsonProperty("OrderId")]
    public string OrderId { get; set; }
}