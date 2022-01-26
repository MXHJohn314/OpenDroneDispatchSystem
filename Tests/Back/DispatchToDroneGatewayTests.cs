namespace Tests.Back;

public class DispatchToDroneGatewayTests
{
    // [Fact]
    // public async Task init_drone_should_return_dto()
    // {
    //     var handleFactory = new HttpHandlerFactory();
    //     handleFactory.jsonAll(Constants.TestInitDroneRequest);
    //     var handle = handleFactory.GetHttpMessageHandler();
    //     var mockFleet = new Mock<IFleetRepository>();
    //     var dispatchGateway = new DispatchToSimDroneGateway(mockFleet.Object);
    //     dispatchGateway.ChangeHandle(handle);
    //     var response = await dispatchGateway.InitDrone(Constants.TestInitDroneRequest);
    //     response.Should().NotBeNull();
    //     response.Should().BeEquivalentTo(Constants.TestInitDroneResponse);
    // } Untestable, mocked gateway hidden by basegateway's real one

    // [Fact]
    // public async Task assign_fleet_should_return_dto()
    // {
    //     var testFleetDb = new List<DroneRecord>(){Constants.TestRecord};
    //     var handleFactory = new HttpHandlerFactory();
    //     handleFactory.jsonAll(Constants.TestAssignFleetRequest);
    //     var handle = handleFactory.GetHttpMessageHandler();
    //     var mockFleet = new Mock<IFleetRepository>();
    //     mockFleet.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(testFleetDb.FindLast(x => x.DroneId == y)));
    //     var dispatchGateway = new DispatchToSimDroneGateway(mockFleet.Object);
    //     dispatchGateway.ChangeHandle(handle);
    //     var response = await dispatchGateway.AssignFleet(Constants.TestAssignFleetRequest);
    //     response.Should().NotBeNull();
    //     response.Should().BeEquivalentTo(Constants.TestAssignFleetResponse);
    // } Untestable, mocked gateway hidden by basegateway's real one

    // [Fact]
    // public async Task assign_delivery_should_return_dto()
    // {
    //     var testFleetDb = new List<DroneRecord>(){Constants.TestRecord};
    //     var handleFactory = new HttpHandlerFactory();
    //     handleFactory.jsonAll(Constants.TestAssignDeliverRequest);
    //     var handle = handleFactory.GetHttpMessageHandler();
    //     var mockFleet = new Mock<IFleetRepository>();
    //     mockFleet.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(testFleetDb.FindLast(x => x.DroneId == y)));
    //     var dispatchGateway = new DispatchToSimDroneGateway(mockFleet.Object);
    //     dispatchGateway.ChangeHandle(handle);
    //     var response = await dispatchGateway.AssignDelivery(Constants.TestAssignDeliverRequest);
    //     response.Should().NotBeNull();
    //     response.Should().BeEquivalentTo(Constants.TestAssignDeliveryResponse);
    // } Untestable, mocked gateway hidden by basegateway's real one
}