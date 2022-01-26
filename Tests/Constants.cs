using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;

namespace Tests;

public class Constants
{
    public const string DroneIp = "localhost:5000";
    public const string DispatcherIp = "localhost:4000";
    public const string Url = "https://FlyingPizza.com";
    public const string DroneId = "abcdefabcdefabcdefabcdef";
    public const string TestOrderId = "123456123456123456123456";
    public const string CHARGING = "Charging";
    public static readonly object[] TestItems = Array.Empty<object>();
    public static readonly OrderState TestOrderState = OrderState.Waiting;
    
    private static readonly GeoLocation
        TestDeliveryLocation = new()
        {
            Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
        },
        TestHomeLocation = new()
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00561147600774m
        };

    private const string
        TestDeliveryAddress = "yo mama",
        TestCustomerName = "bobby";

    public static readonly GeoLocation HomeLocation = new()
    {
        Latitude = 0.00001m,
        Longitude = 0.00001m
    };

    public static readonly GeoLocation Destination = new()
    {
        Latitude = 0.00018m,
        Longitude = 0.00011m
    };
    private static readonly DateTime TestTimeDelivered = DateTime.UtcNow;

    
    public static DroneRecord TestRecord = new()
    {
        DispatchUrl = Url,
        Id = DroneId,
        DroneUrl = DispatcherIp,
        HomeLocation = HomeLocation,
        Destination = Destination,
        CurrentLocation = HomeLocation,
        OrderId = TestOrderId,
        State = DroneState.Ready,
        DroneId = DroneId
    };
    public static readonly Order TestOrder = new()
    {
        CustomerName = TestCustomerName,
        DeliveryAddress = TestDeliveryAddress,
        DeliveryLocation = TestDeliveryLocation,
        DroneId = TestRecord.DroneId,
        Id = TestOrderId,
        Items = TestItems,
        OrderId = TestOrderId,
        State = TestOrderState,
        TimeDelivered = TestTimeDelivered,
        TimeOrdered = TestTimeDelivered
        
    };

    public static readonly Dictionary<string, string> TestStringDict = new()
    {
        ["something"] = "something"
    };

    public static readonly HttpResponseMessage Httpok = new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent("")
    };
    
    public static readonly CompleteOrderRequest TestCompleteOrderRequest = new()
    {
        OrderId = TestOrderId,
        Time = TestTimeDelivered
    };
    public static string TelloIp = "192.168.10.1";
    
        public static readonly AssignDeliveryRequest TestAssignDeliverRequest = new()
        {
            DroneId = DroneId,
            OrderId = TestOrderId,
            OrderLocation = Destination
        };

        public static readonly AssignDeliveryResponse TestAssignDeliveryResponse = new()
        {
            DroneId = DroneId,
            OrderId = TestOrderId,
            Success = true
        };

        public static readonly UpdateDroneStatusResponse TestUpdateResponse = new()
        {
            DroneId = DroneId,
            IsCompletedSuccessfully = true
        };
        public static string TestPingString = "I am pinging!";
        
        public static BaseDto TestPingDto = new()
        {
            Message = TestPingString
        };
        
        public static readonly AddDroneRequest TestAddDroneRequest = new()
        {
            DispatchUrl = TestRecord.DispatchUrl,
            DroneId = TestRecord.DroneId,
            DroneUrl = TestRecord.DroneUrl,
            HomeLocation = TestRecord.HomeLocation
        };

        public static readonly AddDroneResponse TestAddDroneResponse = new()
        {
            DroneId = TestRecord.DroneId,
            Success = true
        };

        public static readonly InitDroneResponse TestInitDroneResponse = new()
        {
            DroneId = TestRecord.DroneId,
            Okay = true
        };

        public static readonly AssignFleetResponse TestAssignFleetResponse = new()
        {
            DroneId = TestRecord.DroneId,
            FirstState = DroneState.Ready,
            IsInitializedAndAssigned = true

        };

        public static readonly EnqueueOrderRequest TestEnqueueOrderRequest = new()
        {
            OrderId = TestOrderId,
            OrderLocation = TestOrder.DeliveryLocation
        };
        
        public static readonly EnqueueOrderResponse TestEnqueueOrderResponse = new()
        {
            IsAssigned = true
        };

        public static readonly InitDroneRequest TestInitDroneRequest = new()
        {
            DroneId = TestRecord.DroneId,
            DroneUrl = TestRecord.DroneUrl
        };

        public static readonly AssignFleetRequest TestAssignFleetRequest = new()
        {
            DispatchUrl = TestRecord.DispatchUrl,
            DroneId = TestRecord.DroneId,
            DroneUrl = TestRecord.DroneUrl,
            HomeLocation = TestRecord.HomeLocation
        };

        public static readonly CompleteOrderResponse TestCompleteOrderResponse = new()
        {
            IsAcknowledged = true
        };
    

        public static DroneUpdate TestUpdate = new()
        {
            CurrentLocation = TestRecord.CurrentLocation,
            Destination = TestRecord.Destination,
            DroneId = TestRecord.DroneId,
            OrderId = TestRecord.OrderId,
            State = TestRecord.State
        };

        public static readonly DroneRecord TestRecordDead = new()
        {
            CurrentLocation = TestRecord.CurrentLocation,
            Destination = TestRecord.Destination,
            DispatchUrl = TestRecord.DispatchUrl,
            DroneId = TestRecord.DroneId,
            DroneUrl = TestRecord.DroneUrl,
            HomeLocation = TestRecord.HomeLocation,
            Id = TestRecord.Id,
            OrderId = TestRecord.OrderId,
            State = DroneState.Dead
        };

        public static readonly RecoveryRequest TestReviveRequest = new()
        {
            Record = TestRecordDead
        };

        public static readonly DroneRecord TestRecordReturnHome = new()
        {
            CurrentLocation = TestRecord.CurrentLocation,
            Destination = TestRecord.Destination,
            DispatchUrl = TestRecord.DispatchUrl,
            DroneId = TestRecord.DroneId,
            DroneUrl = TestRecord.DroneUrl,
            HomeLocation = TestRecord.HomeLocation,
            Id = TestRecord.Id,
            OrderId = TestRecord.OrderId,
            State = DroneState.Returning
        };
}