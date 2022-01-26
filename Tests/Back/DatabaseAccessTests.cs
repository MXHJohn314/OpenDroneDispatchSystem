using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Back;

public class DatabaseAccessTests
{
    [Fact]
    public async Task get_fleet_should_get_list_of_drone_records()
    {
        var mockOrders = new Mock<IOrdersRepository>();
        var mockFleet = new Mock<IFleetRepository>();
        var testDb = new List<DroneRecord>(){Constants.TestRecord};
        mockFleet.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(testDb));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrders.Object);
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
        var result = await access.GetFleet();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testDb);
    }
    
    // [Fact]
    // public async Task create_order_should_place_order_given_return_empty_obj_for_now()
    // {
    //     var handleFactory = new HttpHandlerFactory();
    //     handleFactory.StringAll();
    //     var handle = handleFactory.GetHttpMessageHandler();
    //     var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
    //     var mockFleet = new Mock<IFleetRepository>();
    //     var mockOrders = new Mock<IOrdersRepository>();
    //     LocationParser.ChangeHandler(handle);
    //     mockOrders.Setup(x => x.CreateAsync(Constants.TestOrder)).Verifiable();
    //     mockOrders.Setup(x => x.CreateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
    //     var mockODDS = new Mock<IODDSSettings>();
    //     mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
    //     mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrders.Object);
    //     var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
    //     var result = await access.CreateOrder(Constants.TestOrder);
    //     mockOrders.Verify();
    //     result.Should().NotBeNull();
    //     result.IsCompletedSuccessfullly.Should().BeFalse();
    //     // Not Testable, HttpSocket can't be mocked here
    // }
    
    [Fact]
    public void get_drone_should_pull_a_record()
    {
        var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
        var mockFleet = new Mock<IFleetRepository>();
        var mockOrders = new Mock<IOrdersRepository>();
        var testDb = new List<DroneRecord>() {Constants.TestRecord};
        mockFleet.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string x) => Task.FromResult(testDb.Find(y => y.DroneId == x)));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrders.Object);
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
        var result = access.GetDrone(Constants.DroneId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Constants.TestRecord);
    }
    
    [Fact]
    public void get_order_should_pull_an_order()
    {
        var mockFleet = new Mock<IFleetRepository>();
        var mockOrders = new Mock<IOrdersRepository>();
        var testDb = new List<Order>() {Constants.TestOrder};
        mockOrders.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string x) => Task.FromResult(testDb.Find(y => y.Id == x)));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrders.Object);
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
        var result = access.GetOrderById(Constants.TestOrderId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Constants.TestOrder);
    }
}