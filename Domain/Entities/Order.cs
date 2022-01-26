using System;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[BsonDiscriminator("Order")]
public class Order : BaseEntity
{
    [BsonElement("DroneId")] public string DroneId { get; set; }

    [BsonElement("OrderId")] public string OrderId { get; set; }

    [BsonElement("State")]
    [JsonPropertyName("State")]
    public OrderState State { get; set; }

    [BsonElement("Items")]
    [JsonPropertyName("Items")]
    public object[] Items { get; set; }

    [BsonElement("CustomerName")]
    [JsonPropertyName("CustomerName")]
    public string CustomerName { get; set; }

    [BsonElement("DeliveryAddress")]
    [JsonPropertyName("DeliveryAddress")]
    public string DeliveryAddress { get; set; }

    [BsonElement("DeliveryLocation")]
    [JsonPropertyName("DeliveryLocation")]
    public GeoLocation DeliveryLocation { get; set; }

    [BsonElement("TimeOrdered")]
    [JsonPropertyName("TimeOrdered")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime TimeOrdered { get; set; }

    [BsonElement("TimeDelivered")]
    [JsonPropertyName("TimeDelivered")]
    [BsonRepresentation(BsonType.DateTime)]
    [BsonIgnoreIfNull]
    public DateTime? TimeDelivered { get; set; }


    [BsonElement("HasBeenDelivered")]
    [JsonPropertyName("HasBeenDelivered")]
    [BsonIgnoreIfNull]
    public bool HasBeenDelivered => TimeDelivered != null;

    public OrderUpdate Update()
    {
        return new()
        {
            DroneId = DroneId,
            HasBeenDelivered = HasBeenDelivered,
            OrderId = OrderId,
            State = State,
            TimeDelivered = TimeDelivered
        };
    }
}