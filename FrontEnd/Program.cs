using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace FrontEnd;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");

        var dbAccessUrl = builder.Configuration.GetValue<string>("REMOTE_DB_URL");
        // var dbAccessUrl = builder.Configuration.GetValue<string>("LOCAL_DB_URL");

        builder.Services.AddSingleton(_ => new FrontEndToDatabaseGateway(dbAccessUrl));
        
        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        
        builder.Services.AddSingleton(new GlobalDataSvc());

        builder.Services.AddScoped<DialogService>();

        await builder.Build().RunAsync();
    }
}