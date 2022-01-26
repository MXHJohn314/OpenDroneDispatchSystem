using Bunit;
using FrontEnd.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radzen;
using Xunit;
using TestContext = Bunit.TestContext;
using Radzen;
using Assert = Xunit.Assert;

namespace Tests.Front;

[TestClass]
public class OrderPageTest
{
    [Fact]
    public static void TestOrderPage()
    {

        using var cxt = new TestContext();
        cxt.JSInterop.SetupVoid("initGeocoder");
        cxt.Services.AddSingleton<DialogService>();
        // cxt.Services.AddScoped(_=> new FrontEndToDatabaseGateway("DatabaseAccess"));
        // var check = cxt.RenderComponent<FrontEnd.Pages.OrderPages.OrderPage>();
        //
        // var connectionBar = check.Find(".quick-look").LastChild.ToMarkup();
        // Assert.Equal(connectionBar,
        //     "\r\n<div class=\"connection-bar\" data-hover=\"Database: Connected\" b-2mtgbf0gm5=\"\">\r\n  " +
        //     "<span class=\"oi oi-circle-check\" style=\"color: #08af08\" b-2mtgbf0gm5=\"\"></span>\r\n</div>" );
        //
        //
        // var customerName = check.Find("#CUST_NAME");
        // var custAddress = check.Find("#ADDRESS");
        //
        //
        // var cancelOrder = check.Find("#CANCEL_ORDER");
        // var cancelOrderBtn = check.Find("#CANCEL_ORDER_BTN");
        //
        // var orderFleet = check.Find("#DRONE_FLEET_CARD");
        // var orderFleetCount = orderFleet.Children;
        // Assert.Equal(15, orderFleetCount.Length);
        // foreach (var order in check.FindAll(".drone-status"))
        // {
        //     var exit = check.Find("#EXIT_BTN");
        //     Assert.Equal(exit.TextContent, "X");
        // }
        //
        // var droneURL = check.Find("#DRONE_URL");
        // var droneURL2 = check.Find("#DRONE_URL2");
        //
        // customerName.Change("Bob");
        // custAddress.Change("2300 Steele St, Denver, CO 80205");
        // cancelOrder.Change("1");
        // var createOrderBtn = check.Find("#ORDER_CREATE_BTN");
        // createOrderBtn.Click();
        // droneURL.Change("http://test1:87");
        // var addDroneBtn = check.Find("#ADD_DRONE_BTN");
        // addDroneBtn.Click();
        // droneURL2.Change("http://test1:87");
        // var removeDroneBtn = check.Find("#REMOVE_DRONE_URL");
        
        
    }
}