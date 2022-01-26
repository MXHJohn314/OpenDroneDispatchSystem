using System;
using System.Linq;
using AngleSharp.Text;
using Bunit;
using FrontEnd.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using TestContext = Bunit.TestContext;
using Radzen;
using Xunit.Abstractions;
using Assert = Xunit.Assert;


namespace Tests.Front;

[TestClass]
public class FleetViewTest
{
    [Fact]
    public static void TestFleetDrone()
    {
        using var cxt = new TestContext();
        cxt.Services.AddSingleton<DialogService>();
        cxt.Services.AddSingleton<GlobalDataSvc>();
        // cxt.Services.AddScoped(_=> new FrontEndToDatabaseGateway("DatabaseAccess"));
        // var check = cxt.RenderComponent<FrontEnd.Pages.FleetPages.FleetView>();
        //
        // var connectionBar = check.Find(".quick-look").LastChild.ToMarkup();
        // Assert.Equal(connectionBar,
        //     "\r\n<div class=\"connection-bar\" data-hover=\"Database: Connected\" b-j4hm45p6x7=\"\">\r\n  " +
        //     "<span class=\"oi oi-circle-check\" style=\"color: #08af08\" b-j4hm45p6x7=\"\"></span>\r\n</div>" );
        //
        // var droneCountString = check.Find("b").TextContent;
        // var droneCount = droneCountString.Split().Last();
        // var droneObjCount = check.Find(".drone-fleet").Children.Length;
        // Assert.Equal(int.Parse(droneCount), droneObjCount);
        //
        // var droneBtn = check.FindAll(".rz-button rz-button-md btn-primary rz-button-icon-only");
        // foreach (var drone in droneBtn)
        // {
        //     drone.Click();
        //     var detail = check.Find(".rz-dialog-title").TextContent;
        //     Assert.Equal(detail, "DetailedSimDrone");
        //     var closeBtn = check.Find("#ADASDADASD");
        //     closeBtn.Click();
        // }


    }
}