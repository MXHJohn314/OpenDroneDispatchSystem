using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using FluentAssertions;
using Moq;
using SimDrone;
using SimDrone.Controllers;
using Xunit;

namespace Tests.Back;

public class RealDroneTests
{

    [Fact]
    public void drone_should_get_positive_route_positive_dest()
    {
        var mockedGateway = new Mock<IDroneToDispatchGateway>();
        var start = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = 0.0m
        };
        var end = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = 0.0002m
        };
        var telloDrone = new TelloDrone(new DroneRecord
        {
            CurrentLocation = start,
            Destination = end,
            DispatchUrl = "unused",
            DroneId = "unused",
            DroneUrl = Constants.TelloIp,
            HomeLocation = start,
            Id = Constants.TestOrderId,
            OrderId = Constants.TestOrderId,
            State = DroneState.Ready
        }, mockedGateway.Object,new SimDroneController(), true);
        var route = telloDrone.GetRoute();
        route.Should()
            .NotBeNull();
        foreach (var geoLocation in route)
        {
            geoLocation.Latitude.Should()
                .BeGreaterThanOrEqualTo(0m);
            geoLocation.Longitude.Should()
                .BeGreaterThanOrEqualTo(0m);
        }
    }
    
    
    public void drone_should_get_negative_route_negative_dest()
    {
        var mockedGateway = new Mock<IDroneToDispatchGateway>();
        var start = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = 0.0m
        };
        var end = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = -0.0002m
        };
        var telloDrone = new TelloDrone(new DroneRecord
        {
            CurrentLocation = start,
            Destination = end,
            DispatchUrl = "unused",
            DroneId = "unused",
            DroneUrl = Constants.TelloIp,
            HomeLocation = start,
            Id = Constants.TestOrderId,
            OrderId = Constants.TestOrderId,
            State = DroneState.Ready
        },mockedGateway.Object, new SimDroneController(), true);
        var route = telloDrone.GetRoute();
        route.Should()
            .NotBeNull();
        foreach (var geoLocation in route)
        {
            geoLocation.Latitude.Should()
                .BeLessThanOrEqualTo(0m);
            geoLocation.Longitude.Should()
                .BeLessThanOrEqualTo(0m);
        }
    }


    [Fact]
    public void drone_send_command_should_return_true_all_valid_commands()
    {
        var mockedGateway = new Mock<IDroneToDispatchGateway>();
        var commands = new []{ "command", "forward 100", "back 100", "left 100", "right 100", "takeoff", "land"};
        var telloDrone = new TelloDrone(new DroneRecord
        {
            CurrentLocation = Constants.HomeLocation,
            Destination = Constants.Destination,
            DispatchUrl = "unused",
            DroneId = "unused",
            DroneUrl = Constants.TelloIp,
            HomeLocation = Constants.HomeLocation,
            Id = Constants.TestOrderId,
            OrderId = Constants.TestOrderId,
            State = DroneState.Ready
        }, mockedGateway.Object,new SimDroneController());
        foreach (var command in commands)
        {
            var response = telloDrone.send_command(command, true);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(Task.FromResult(true));
        }

    }

    [Fact]
    public void drone_should_deliver_2_meters()
    {
        var mockedGateway = new Mock<IDroneToDispatchGateway>();
        var start = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = 0.0m
        };
        var end = new GeoLocation
        {
            Latitude = 0.0m,
            Longitude = 0.0002m
        };
        var telloDrone = new TelloDrone(new DroneRecord
        {
            CurrentLocation = start,
            Destination = end,
            DispatchUrl = "unused",
            DroneId = "unused",
            DroneUrl = Constants.TelloIp,
            HomeLocation = start,
            Id = Constants.TestOrderId,
            OrderId = Constants.TestOrderId,
            State = DroneState.Ready
        },mockedGateway.Object, new SimDroneController(), true);
        telloDrone.DeliverOrder(new AssignDeliveryRequest
        {
            DroneId = "unused",
            OrderId = Constants.TestOrderId,
            OrderLocation = end
        });
        telloDrone.Route.Should().Contain(end);
    }
}