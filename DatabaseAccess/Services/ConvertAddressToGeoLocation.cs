using System.Web;
using Domain.DTO;
using Newtonsoft.Json.Linq;

namespace DatabaseAccess.Services;


public static class LocationParser
{
    private const string BaseUrl = "https://maps.googleapis.com/maps/api";
    private static readonly HttpClient HttpClient = new();

    // Use the api key and street address to form a full google maps geolocation request url.
    // Return a Geolocation with the response's coordinates.
    public static async Task<GeoLocation> Parse(string apiKey, string streetAddress)
    {
        Console.WriteLine($"\n\n\n\nParsing address from request with \"{streetAddress}\"\n\n\n\n");
        var requestUrl = $"{BaseUrl}/geocode/json?address={HttpUtility.UrlEncode(streetAddress)},&key={apiKey}";
        var response = await HttpClient.GetStringAsync(requestUrl);
        Console.Write("RETURNED: " + response);
        var js = JObject.Parse(response);
        var Longitude = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lng"]) ?? string.Empty);
        var Latitude = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lat"]) ?? string.Empty);
        return new GeoLocation {Latitude, Longitude};
    }
}
