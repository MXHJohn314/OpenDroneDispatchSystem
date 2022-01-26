using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDatabaseGateway : BaseGateway<App>
{
    public FrontEndToDatabaseGateway(string dbUrl)
    {
        DbUrl = dbUrl;
    }

    private string DbUrl { get; }

    public async Task<List<DroneRecord>> GetFleet()
    {
        var s = $"SendMessageGet<List<DroneRecord>>({DbUrl}/GetFleet";
        var response = await SendMessageGet<List<DroneRecord>>($"{DbUrl}/GetFleet");
        Console.WriteLine($"{s} => {string.Join("\n", response)}");
        return response;
    }

    public async Task<List<Order>> GetOrders()
    {
        var response = await SendMessageGet<List<Order>>($"{DbUrl}/GetOrders");
        return response;
    }
    public async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request)
    {
        Console.Write($"before FrontEndToDatabaseGateway.SendMessagePost, request = {request}");
        return await SendMessagePost<CreateOrderRequest, CreateOrderResponse>($"{DbUrl}/CreateOrder", request);
    }

    public async Task<CancelDeliveryResponse> CancelOrder(CancelDeliveryRequest request)
        => await SendMessagePost<CancelDeliveryRequest, CancelDeliveryResponse>($"{DbUrl}/CancelOrder", request);

    public async Task<DroneRecord> GetDrone(string id)
    {
        return await SendMessageGet<string, DroneRecord>($"{DbUrl}/GetDrone", id);
    }

    public async Task<Order> GetOrder(string id)
    {
        return await SendMessageGet<string, Order>($"{DbUrl}/GetOrder", id);
    }

    public async Task<string> GetApiKey()
    {
        return await SendMessageGet<string>($"{DbUrl}/GetApiKey");
    }

    public async Task<GeoLocation> GetHomeLocation()
    {
        Console.WriteLine($"FrontEndToDatabaseGateway.GetHomeLocation({DbUrl}/GetHomeLocation)");
        return await SendMessageGet<GeoLocation>($"{DbUrl}/GetHomeLocation");
    }

    public async Task AddDrone(string droneUrl)
    {
        await SendMessagePost($"{DbUrl}/AddDrone", new BaseDto{Message=droneUrl});
    }

    
    public async Task<string> GetDispatchUrl()
    {
        Console.WriteLine($"FrontEndToDatabaseGateway.GetHomeLocation({DbUrl}/GetHomeLocation)");
        return await SendMessageGet<string>($"{DbUrl}/GetDispatchIp");
    }
}