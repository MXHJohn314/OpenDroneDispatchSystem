using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dispatch.Controllers;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Tests.Back;

public class DispatcherControllerTests
{
    
    private List<Order> _testOrderDb = new (){Constants.TestOrder};
    private List<DroneRecord> _testFleetDb = new () {Constants.TestRecord};
    private void FinishOrder()
    {
        _testOrderDb.ForEach(x => x.TimeDelivered = DateTime.Now);
    }

    private void TestAddDrone()
    {
        _testFleetDb.Add(Constants.TestRecord);
    }
    private void TestFlipDrone()
    {
        _testFleetDb.ForEach(x => x.State = DroneState.Ready);
    }

    private void SetupEnvironment()
    {
        _testOrderDb = new List<Order>() {};
        _testFleetDb = new List<DroneRecord>() {};
        Environment.SetEnvironmentVariable("DISPATCH_URL", "bogus");
        Environment.SetEnvironmentVariable("HOME_LATITUDE", "0");
        Environment.SetEnvironmentVariable("HOME_LONGITUDE", "0");
    }
    // [Fact]
    // public void add_drone_should_create_drone_and_respond()
    // {
    //     SetupEnvironment();
    //     var mockFleetRepo = new Mock<IFleetRepository>();
    //     var mockOrderRepo = new Mock<IOrdersRepository>();
    //     var mockGateway = new Mock<IDispatchToSimDroneGateway>();
    //     var mockResult = new Mock<UpdateResult>();
    //     mockGateway.Setup(x => x.InitDrone(It.IsAny<InitDroneRequest>())).Returns(Task.FromResult(Constants.TestInitDroneResponse));
    //     mockGateway.Setup(x => x.AssignFleet(It.IsAny<AssignFleetRequest>()))
    //         .Returns(Task.FromResult(Constants.TestAssignFleetResponse));
    //     mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
    //     mockFleetRepo.Setup(x => x.CreateAsync(It.IsAny<DroneRecord>())).Returns(Task.CompletedTask).Callback(TestAddDrone);
    //     mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockResult.Object)).Callback(TestAddDrone);
    //     var mockODDS = new Mock<IODDSSettings>();
    //     mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
    //     mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object); 
    //     var dispatcher = new DispatchController(mockODDS.Object);
    //     dispatcher.ChangeGateway(mockGateway.Object);
    //     // Have to make new record since it mutates reference given
    //     var resultTask = dispatcher.AddDrone(new DroneRecord
    //     {
    //         CurrentLocation = Constants.TestRecord.CurrentLocation,
    //         Destination = Constants.TestRecord.Destination,
    //         DispatchUrl = Constants.TestRecord.DispatchUrl,
    //         DroneId = Constants.TestRecord.DroneId,
    //         DroneUrl = Constants.TestRecord.DroneUrl,
    //         HomeLocation = Constants.TestRecord.HomeLocation,
    //         Id = Constants.TestRecord.Id,
    //         OrderId = Constants.TestRecord.OrderId,
    //         State = Constants.TestRecord.State
    //     });
    //     resultTask.Wait();
    //     _testFleetDb.Count.Should().BeGreaterThan(0);
    //     // One of the only non-mutated members
    //     _testFleetDb[0].DroneUrl.Should().BeEquivalentTo(Constants.TestAddDroneRequest.DroneUrl);
    // } // Dereferences Mock of drone at some point when ran async

    [Fact]
    public void complete_valid_order_should_be_true()
    {
        SetupEnvironment();
        var mockResult = new Mock<UpdateResult>(); // nasty Mongo object mocked for use to ignore update
        mockResult.Setup(x => x.IsAcknowledged).Returns(true);
        _testOrderDb.Add(Constants.TestOrder);
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockOrderRepo.Setup(x => x.UpdateAsync(It.IsAny<OrderUpdate>()))
            .Returns(Task.FromResult(mockResult.Object)).Callback(FinishOrder);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        var responseTask = dispatcher.CompleteOrder(Constants.TestCompleteOrderRequest);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().BeEquivalentTo(Constants.TestCompleteOrderResponse);
    }

    [Fact]
    public void get_drone_by_id_should_get_drone_record()
    {
        SetupEnvironment();
        _testFleetDb.Add(Constants.TestRecord);
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockFleetRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testFleetDb.FindLast(x => x.DroneId == y)));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.GetDroneById(Constants.TestRecord.DroneId);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Value.Should().NotBeNull();
        response.Value.Should().BeEquivalentTo(Constants.TestRecord);
    }
    
    [Fact]
    public void revive_should_change_db_record_to_ready()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // record made this way since updateDrone mutates by reference
        var deadRecord = new DroneRecord
        {
            CurrentLocation = Constants.TestRecordDead.CurrentLocation,
            Destination = Constants.TestRecordDead.Destination,
            DispatchUrl = Constants.TestRecordDead.DispatchUrl,
            DroneId = Constants.TestRecordDead.DroneId,
            OrderId = Constants.TestRecordDead.OrderId,
            State = Constants.TestRecordDead.State,
            DroneUrl = Constants.TestRecordDead.DroneUrl,
            HomeLocation = Constants.TestRecordDead.HomeLocation,
            Id = Constants.TestRecordDead.Id
        };
        _testFleetDb.Add(deadRecord);
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        var responseTask = dispatcher.Revive(deadRecord);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().BeFalse();
        // Unsuccessful instead of successful being the return is interesting
        _testFleetDb.Should().Contain(Constants.TestRecord);
        _testFleetDb.Should().NotContain(Constants.TestRecordDead);
    }
    
    [Fact]
    public void assign_check_should_pull_order_and_drone_and_initiate_delivery()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        _testOrderDb.Add(Constants.TestOrder);
        _testFleetDb.Add(Constants.TestRecord);
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockGateway.Setup(x => x.AssignDelivery(It.IsAny<AssignDeliveryRequest>())).Returns(Task.FromResult(Constants.TestAssignDeliveryResponse));
        mockGateway.Setup(x => x.HealthCheck(It.IsAny<string>())).Returns(Task.FromResult(true));
        mockFleetRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testFleetDb.FindLast(x => x.DroneId == y)));
        mockOrderRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testOrderDb.FindLast(x => x.DroneId == y)));
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockOrderRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testOrderDb));
        mockOrderRepo.Setup(x => x.UpdateAsync(It.IsAny<OrderUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object));
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.AssignmentCheck(Constants.TestPingDto);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestPingDto);
    }

    [Fact]
    public void post_init_status_should_update_starting_fleet_db()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // record made this way since updateDrone mutates by reference
        var deadRecord = new DroneRecord
        {
            CurrentLocation = Constants.TestRecordDead.CurrentLocation,
            Destination = Constants.TestRecordDead.Destination,
            DispatchUrl = Constants.TestRecordDead.DispatchUrl,
            DroneId = Constants.TestRecordDead.DroneId,
            OrderId = Constants.TestRecordDead.OrderId,
            State = Constants.TestRecordDead.State,
            DroneUrl = Constants.TestRecordDead.DroneUrl,
            HomeLocation = Constants.TestRecordDead.HomeLocation,
            Id = Constants.TestRecordDead.Id
        };
        _testFleetDb.Add(deadRecord);
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockMongoQuery.Setup(x => x.IsAcknowledged).Returns(true);
        mockMongoQuery.Setup(x => x.ModifiedCount).Returns(_testFleetDb.Count);
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.PostInitialStatus(Constants.TestUpdate);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
        _testFleetDb.Should().Contain(Constants.TestRecord);
        _testFleetDb.Should().NotContain(Constants.TestRecordDead);
    }

    [Fact]
    public void update_drone_status_should_change_fleet_db()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // record made this way since updateDrone mutates by reference
        var deadRecord = new DroneRecord
        {
            CurrentLocation = Constants.TestRecordDead.CurrentLocation,
            Destination = Constants.TestRecordDead.Destination,
            DispatchUrl = Constants.TestRecordDead.DispatchUrl,
            DroneId = Constants.TestRecordDead.DroneId,
            OrderId = Constants.TestRecordDead.OrderId,
            State = Constants.TestRecordDead.State,
            DroneUrl = Constants.TestRecordDead.DroneUrl,
            HomeLocation = Constants.TestRecordDead.HomeLocation,
            Id = Constants.TestRecordDead.Id
        };
        _testFleetDb.Add(deadRecord); 
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockMongoQuery.Setup(x => x.IsAcknowledged).Returns(true);
        mockMongoQuery.Setup(x => x.ModifiedCount).Returns(_testFleetDb.Count);
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetOrdersCollection()).Returns(mockOrderRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.UpdateDroneStatus(Constants.TestUpdate);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
    }
    
}
