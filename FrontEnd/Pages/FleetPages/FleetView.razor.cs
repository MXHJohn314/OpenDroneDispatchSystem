using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.FleetPages;

partial class FleetView : ComponentBase
{
    public string color;
    public bool connection;
    public DroneRecord[] Fleet;
    public int size;

    [Inject] public IJSRuntime JsRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Fleet = (await FrontEndToDatabaseGateway.GetFleet()).ToArray();
            size = Fleet.Length;
            connection = true;
        }
        catch(Exception e)
        {
            Console.WriteLine($"{e}");
        }
    }

    public async Task GoToDrone(DroneRecord drone)
    {
        globalData.currDrone = drone;
        await dialogService.OpenAsync<DetailedDrone>("View Drone");
    }

    public string Color(DroneRecord drone)
    {
        return drone.State.GetColor();
    }
}