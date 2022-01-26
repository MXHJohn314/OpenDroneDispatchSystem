using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DTO;

public class OrderUpdate
{
    [BsonElement("OrderId")]
    [JsonProperty("OrderId")]
    public string OrderId { get; set; }

    [BsonElement("State")]
    [JsonProperty("State")]
    public OrderState State { get; set; }

    [BsonElement("TimeDelivered")]
    [JsonProperty("TimeDelivered")]
    public DateTime? TimeDelivered { get; set; }

    [BsonElement("HasBeenDelivered")]
    [JsonProperty("HasBeenDelivered")]
    public bool HasBeenDelivered { get; set; }

    [BsonElement("DroneId")]
    [JsonProperty("DroneId")]
    public string DroneId { get; set; }
}