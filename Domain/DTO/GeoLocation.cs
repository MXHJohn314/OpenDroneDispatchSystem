using System;
using System.Text.Json.Serialization;
using DecimalMath;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO;

public class GeoLocation
{
    private const decimal ToleranceInMeters = 10m;

    [BsonElement("Latitude")]
    [JsonPropertyName("Latitude")]
    [BsonRepresentation(BsonType.String)]
    public decimal Latitude { get; set; }

    [BsonElement("Longitude")]
    [JsonPropertyName("Longitude")]
    [BsonRepresentation(BsonType.String)]
    public decimal Longitude { get; set; }


    public override bool Equals(object? o)
    {
        return o is GeoLocation other && HaversineInMeters(other) < ToleranceInMeters;
    }

    // {
    //     return o is GeoLocation other &&
    //            Math.Abs(Latitude - other.Latitude) < ToleranceInMeters
    //            && Math.Abs(Longitude - other.Longitude) < ToleranceInMeters;
    // }
    private decimal HaversineInMeters(GeoLocation other)
    {
        decimal otherLongitude = other.Longitude,
            otherLatitude = other.Latitude;
        var d1 = Latitude * (DecimalEx.Pi / 180.0m);
        var num1 = Longitude * (DecimalEx.Pi / 180.0m);
        var d2 = otherLatitude * (DecimalEx.Pi / 180.0m);
        var num2 = otherLongitude * (DecimalEx.Pi / 180.0m) - num1;
        var d3 = DecimalEx.Pow(DecimalEx.Sin((d2 - d1) / 2.0m), 2.0m) + DecimalEx.Cos(d1) * DecimalEx.Cos(d2) *
            DecimalEx.Pow(DecimalEx.Sin(num2 / 2.0m), 2.0m);
        return 6376500.0m * 2.0m * DecimalEx.ATan2(DecimalEx.Sqrt(d3), DecimalEx.Sqrt(1.0m - d3));
    }

    public override string ToString()
    {
        return $"{{Latitude:{Latitude},Longitude:{Longitude}}}";
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }


    public GeoLocation OffSet(GeoLocation routeLocation)
    {
        return new()
        {
            Latitude = routeLocation.Latitude + Latitude,
            Longitude = routeLocation.Longitude + Longitude
        };
    }


    public bool IsBetween(GeoLocation location1, GeoLocation location2)
    {
        return ((location1.Longitude >= Longitude && Longitude >= location2.Longitude) ||
                (location2.Longitude >= Longitude && Longitude >= location1.Longitude)) &&
               ((location1.Latitude >= Latitude && Latitude >= location2.Latitude) ||
                (location2.Latitude >= Latitude && Latitude >= location1.Latitude));
    }

    public string ToCsv() => $"{Latitude},{Longitude}";
}