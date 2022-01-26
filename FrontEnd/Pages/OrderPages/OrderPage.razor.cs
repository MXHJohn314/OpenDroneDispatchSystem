using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.OrderPages;

partial class OrderPage : ComponentBase
{
    public string DeliveryAddress;
    public string CustomerName;
    public string DroneUrl;
    public Order[] Orders;

    public bool connection;

    public Order selectedOrder;

    public string visibility = "hidden";

    public string orderToCancel;

    public string defaultText ="";
    public DroneRecord[] Fleet;
    
    protected override async Task OnInitializedAsync()
    {
        try
        {          
            await GetOrders();
            await GetFleet();
            connection = true;
        }
        catch
        {

        }
    } 

    public async Task GetOrders()
    {
        Orders = (await DatabaseGateway.GetOrders()).ToArray();
    }
    public async Task GetFleet()
    {
        Fleet = (await DatabaseGateway.GetFleet()).ToArray();
    }

    private async Task AddDrone()
    {
        await DatabaseGateway.AddDrone(DroneUrl);
    }

    public async Task MakeOrder()
    {
        var orderId = BaseEntity.GenerateNewId();
        await DatabaseGateway.CreateOrder(new CreateOrderRequest
        {
            OrderId = orderId,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = null,
            DeliveryAddress = DeliveryAddress,
            DroneId = DroneUrl,
            State = OrderState.Waiting
        });
        await GetOrders(); 
    }

    public async Task CancelOrder()
    {
        if (!string.IsNullOrEmpty(orderToCancel))
        {
            orderToCancel = selectedOrder.Id;
        }
        defaultText = orderToCancel;
        await GetOrders();
        orderToCancel = null;
        defaultText = "";
    }

    public void OnInfoClose(){
        visibility = "hidden";
        defaultText = "";
        selectedOrder = null;
    }

    public void DisplaySelected(Order selected)
    {
        selectedOrder = selected;
        defaultText = selectedOrder.Id;
        visibility = "visible";
    }

    public string Color(DroneRecord drone)
    {
        return drone.State.GetColor();
    }
    
}