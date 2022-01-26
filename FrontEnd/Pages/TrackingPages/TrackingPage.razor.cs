using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.DTO;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.TrackingPages;

partial class TrackingPage : ComponentBase
{
    private const int RefreshInterval = 2000;
    private Stopwatch _stopwatch;
    private Timer _timer;
    public bool connection;
    public string dropDownLabel;
    public DroneRecord[] filteredDrones;
    public DroneRecord[] Fleet;

    [Inject] public FrontEndToDatabaseGateway _frontEndToDatabaseGateway { get; set; }
    public GeoLocation HomeLocation { get; set; }
    [Inject] public IJSRuntime JsRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Console.WriteLine("_frontEndToDatabaseGateway == null ?" + _frontEndToDatabaseGateway == null);
            HomeLocation = await _frontEndToDatabaseGateway.GetHomeLocation();
            // Console.WriteLine($"Home location is {HomeLocation.Latitude}, {HomeLocation.Longitude}");
            await JsRuntime.InvokeVoidAsync("initGoogleMap", 
                new {lat = HomeLocation.Latitude, lng = HomeLocation.Longitude});
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await DisplayDroneAsync("Delivering");
        await Temp();
        await UpdateDroneMarkers();
    }

    protected Task Temp()
    {
        _timer = new Timer(MarkerUpdateCallback, null, 0, RefreshInterval);
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        return Task.CompletedTask;
    }

    protected async Task DisplayDroneAsync(string filter)
    {
        try
        {
            dropDownLabel = filter;
            Fleet = (await _frontEndToDatabaseGateway.GetFleet()).ToArray();
            filteredDrones = Fleet.Where(record => record.State.ToString() == filter).ToArray();
            connection = true;
        }
        catch
        {
            // ignored
        }
    }

    private async void MarkerUpdateCallback(object _)
    {
        await UpdateDroneMarkers();
    }

    private async Task UpdateDroneMarkers()
    {
        var droneRecords = await _frontEndToDatabaseGateway.GetFleet();
        var currentMarkers = droneRecords
            .Select(x => new JsTriangle(x.CurrentLocation, x.Direction,$"Drone {x.BadgeNumber}", x.State.GetColor(), x.Destination))
            .ToDictionary(x => x.title, x => x);
        var newDestinations = droneRecords.Where(x => x.State is DroneState.Delivering or DroneState.Returning)
            .Select(x => new JsCircle(x.Destination,  $"Drone {x.BadgeNumber} destination", "#ffffff"))
            .ToDictionary(x => x.title, x => x);
         var newPaths = droneRecords.Where(x => x.State is DroneState.Delivering or DroneState.Returning).Select(x =>
            new JsPath(x.CurrentLocation, x.Destination, $"Drone {x.BadgeNumber} destination", x.State.GetColor()))
             .ToDictionary(x => x.title, x => x);
         
        await JsRuntime.InvokeVoidAsync("updateAll", currentMarkers, newDestinations, newPaths);
        await DisplayDroneAsync(dropDownLabel);
        StateHasChanged(); 
    }

    public string Color(DroneRecord drone)
    {
        return drone.State.GetColor();
    }

    public class JsTriangle
    {
        public JsTriangle(GeoLocation location, decimal bearing, string title, string color, GeoLocation destination)
        {
            lat = location.Latitude;
            lng = location.Longitude;
            destLat = destination.Latitude;
            destLng = destination.Longitude;
            this.color = color;
            this.title = title;
            this.bearing = bearing;
        }
        public decimal destLat { get; set; }
        public decimal destLng { get; set; }
        public decimal bearing { get; set; }
        public string title { get; }
        public decimal lat { get; }
        public decimal lng { get; }
        public string color { get; }
    }

    public class JsCircle
    {
        public JsCircle(GeoLocation location, string title, string color)
        {
            this.title = title;
            lat = location.Latitude;
            lng = location.Longitude;
            this.color = color;
        }

        public string title { get; }
        public decimal lat { get; }
        public decimal lng { get; }
        public string color { get; }
    }
}

public class JsPath
{
    public JsPath(GeoLocation loc1, GeoLocation loc2, string title, string color)
    {
        lat1 = loc1.Latitude;
        lng1 = loc1.Longitude;
        lat2 = loc2.Latitude;
        lng2 = loc2.Longitude;
        this.title = title;
        this.color = color;
    }

    public decimal lat1 { get; }
    public string title { get; }
    public decimal lng2 { get; }
    public string color { get; }
    public decimal lat2 { get; }
    public decimal lng1 { get; }
}