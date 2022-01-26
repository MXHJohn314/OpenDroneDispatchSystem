using System.Threading.Tasks;
using Domain.DTO;
using Domain.GatewayDefinitions;
using FluentAssertions;
using Moq;
using SimDrone;
using SimDrone.Controllers;
using Xunit;

namespace Tests.Back;

public class SimDroneControllerTests
{
    
    [Fact]
    public async Task init_drone_should_return_dto()
    {
        var sim = new SimDroneController();
        var response = await sim.InitDrone(Constants.TestInitDroneRequest);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestInitDroneResponse);
    }

    // [Fact]
    // public async Task assign_fleet_should_create_private_members_and_return_dto()
    // {
    //     var mockedDroneDispatchGateway = new Mock<IDroneToDispatchGateway>();
    //     mockedDroneDispatchGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
    //     var mockedGate = mockedDroneDispatchGateway.Object;
    //     var sim = new SimDroneController();
    //     var drone = new Drone(Constants.TestRecord, mockedGate,sim);
    //     sim.ChangeDrone(drone);
    //     var response = await sim.AssignFleet(Constants.TestAssignFleetRequest);
    //     response.Should().NotBeNull();
    //     response.Should().BeEquivalentTo(Constants.TestAssignFleetResponse);
    // } untestable, as it changes all other tests by swapping mock out with rejoin fleet
    
    
    [Fact]
    public async Task assign_delivery_should_deliver_and_return_dto()
    {
        var sim = new SimDroneController();
        var mockedGateway = new Mock<IDroneToDispatchGateway>();
        mockedGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
        sim.ChangeGateway(mockedGateway.Object);
        sim.ChangeDrone(new Drone(Constants.TestRecord,mockedGateway.Object, sim));
        var response = await sim.AssignDelivery(Constants.TestAssignDeliverRequest);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestAssignDeliveryResponse);
    }

    [Fact]
    public async Task update_drone_status_call_update_and_return_dto()
    {
        var mockedGateway = new Mock<IDroneToDispatchGateway>();
        mockedGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
        var sim = new SimDroneController();
        sim.ChangeGateway(mockedGateway.Object);
        var response = await sim.UpdateDroneStatus(Constants.TestRecord);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
    }
    
    // [Fact]
    // public async Task rejoin_fleet_should_return_inserted_drone_directed_home()
    // {
    //     
    //     
    //     var mockedGateway = new Mock<IDroneToDispatchGateway>();
    //     mockedGateway.Setup(x => x.CompleteDelivery(It.IsAny<CompleteOrderRequest>())).Returns(Task.FromResult(Constants.TestCompleteOrderResponse));
    //     mockedGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
    //     var sim = new SimDroneController();
    //     var testDrone = new Drone(Constants.TestRecordDead, mockedGateway.Object, sim);
    //     testDrone.Id = Constants.TestRecord.Id;
    //     testDrone.OrderId = Constants.TestRecord.OrderId;
    //     testDrone.DispatchUrl = Constants.TestRecord.DispatchUrl;
    //     sim.ChangeDrone(testDrone);
    //     await sim.RejoinFleet(Constants.TestReviveRequest); 
    //     var currentDrone = await sim.HealthCheck(Constants.TestPingDto);
    //     currentDrone.DroneId.Should().BeEquivalentTo(Constants.TestRecord.DroneId);
    //     currentDrone.DroneUrl.Should().BeEquivalentTo(Constants.TestRecord.DroneUrl);
    //     currentDrone.Destination.Should().BeEquivalentTo(Constants.TestRecord.HomeLocation);
    // } Untestable as rejoin fleet swaps out mocked gateways

    [Fact]
    public async Task health_check_should_return_its_own_drone_as_record()
    {
        var mockedGateway = new Mock<IDroneToDispatchGateway>();        
        mockedGateway.Setup(x => x.UpdateDroneStatus(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(Constants.TestUpdateResponse));
        var testDrone = new Drone(Constants.TestRecord,mockedGateway.Object, new SimDroneController());
        testDrone.Id = Constants.TestRecord.Id;
        testDrone.DispatchUrl = Constants.TestRecord.DispatchUrl;
        var sim = new SimDroneController();
        sim.ChangeGateway(mockedGateway.Object);
        sim.ChangeDrone(testDrone);
        var response = await sim.HealthCheck(Constants.TestPingDto);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestRecord);
    }
    
    
}