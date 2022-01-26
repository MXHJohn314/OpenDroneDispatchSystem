using System;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateOrderRequest : BaseDto
{
    private GeoLocation _location;

    public string OrderId { get; set; }
    public string CustomerName {get;set;}
    public string DeliveryAddress{get;set;}
    public string DroneId { get;set; }
    public DateTime TimeOrdered{get;set;}
    // private GeoLocation _location;
    public GeoLocation DeliveryLocation{get;set;}
    public OrderState State { get; set; }
}