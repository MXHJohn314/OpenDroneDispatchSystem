using System;
using System.Net.Http;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions.Common;
using FrontEnd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using TestContext = Bunit.TestContext;
using Radzen;
using Assert = Xunit.Assert;
using FrontEnd;


namespace Tests.Front;

[TestClass]
public class TrackingPageTest
{
    [Fact]
    public static async void TestTrackingPage()
    {

        using var cxt = new TestContext();
        cxt.JSInterop.SetupVoid("initGoogleMap", _ => true);
        // cxt.Services.AddScoped(_=> new FrontEndToDatabaseGateway("DatabaseAccess"));
        // cxt.Services.AddSingleton(new GlobalDataSvc());
        // var check = cxt.RenderComponent<FrontEnd.Pages.TrackingPages.TrackingPage>();
        //
        // var connectionBar = check.Find(".quick-look").LastChild.ToMarkup();
        // Assert.Equal(connectionBar,
        //     "\r\n<div class=\"connection-bar\" data-hover=\"Database: Connected\" b-m2qf942cjn=\"\">\r\n  " +
        //     "<span class=\"oi oi-circle-check\" style=\"color: #08af08\" b-m2qf942cjn=\"\"></span>\r\n</div>" );
        //
        // var mapCheck = check.Find("#map");
        // Assert.False(mapCheck.IsValid());
        //
        // var dropdownBtn = check.Find(".dropdown");
        // dropdownBtn.IsHovered();
        // var dropdownContent = check.Find(".dropdown-content");
        // var readyStatusBtn = check.Find("#ready-status");
        // readyStatusBtn.Click();
        // var droneFleet = check.Find(".drone-fleet");
        // var droneFleetCount = droneFleet.Children.Length;
        // Assert.Equal(5, droneFleetCount);

    }
}