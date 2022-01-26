using System;
using System.Threading.Tasks;
using DecimalMath;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;

namespace Domain;

public class GeoCalculations
{
    private const decimal RadiusEarthKilometres = 6371.01m;
    private const decimal DegToRadFactor = DecimalEx.Pi / 180;
    private const decimal RadToDegFactor = 180 / DecimalEx.Pi;

    public static decimal ToRadians(decimal x)
    {
        return x * DecimalEx.Pi / 180;
    }

    public static decimal ToDegrees(decimal x)
    {
        return x * 180 / DecimalEx.Pi;
    }

    public static decimal DegreesToRadians(decimal degrees)
    {
        return degrees * DegToRadFactor;
    }

    public static decimal RadiansToDegrees(decimal radians)
    {
        return radians * RadToDegFactor;
    }
    public static decimal HaversineInMeters(GeoLocation locationA, GeoLocation locationB)
    {
        decimal longitude = locationA.Longitude,
            latitude = locationA.Latitude,
            otherLongitude = locationB.Longitude,
            otherLatitude = locationB.Latitude;
        var d1 = latitude * (DecimalEx.Pi / 180.0m);
        var num1 = longitude * (DecimalEx.Pi / 180.0m);
        var d2 = otherLatitude * (DecimalEx.Pi / 180.0m);
        var num2 = otherLongitude * (DecimalEx.Pi / 180.0m) - num1;
        var d3 = DecimalEx.Pow(DecimalEx.Sin((d2 - d1) / 2.0m), 2.0m) + DecimalEx.Cos(d1) * DecimalEx.Cos(d2) *
            DecimalEx.Pow(DecimalEx.Sin(num2 / 2.0m), 2.0m);
        return RadiusEarthKilometres * 2.0m * DecimalEx.ATan2(DecimalEx.Sqrt(d3), DecimalEx.Sqrt(1.0m - d3));
    }

    public static GeoLocation GetNextLocation(GeoLocation startLocation, decimal initialBearingInRadians,
        decimal distanceInKilometres)
    {
        var distRatio = distanceInKilometres / RadiusEarthKilometres;
        var distRatioSine = DecimalEx.Sin(distRatio);
        var distRatioCosine = DecimalEx.Cos(distRatio);

        var startLatRad = DegreesToRadians(startLocation.Latitude);
        var startLonRad = DegreesToRadians(startLocation.Longitude);

        var startLatCos = DecimalEx.Cos(startLatRad);
        var startLatSin = DecimalEx.Sin(startLatRad);

        var endLatRads = DecimalEx.ASin(startLatSin * distRatioCosine +
                                        startLatCos * distRatioSine * DecimalEx.Cos(initialBearingInRadians));

        var endLonRads = startLonRad +
                         DecimalEx.ATan2(
                             DecimalEx.Sin(initialBearingInRadians) * distRatioSine * startLatCos,
                             distRatioCosine - startLatSin * DecimalEx.Sin(endLatRads));

        return new GeoLocation
        {
            Latitude = RadiansToDegrees(endLatRads),
            Longitude = RadiansToDegrees(endLonRads)
        };
    }
    public static decimal Abs(decimal x)
    {
        return x > 0 ? x : -x;
    }
    public static decimal Bearing(decimal lat1Degrees, decimal lon1Degrees, decimal lat2Degrees, decimal lon2Degrees)
    {
        var startLat = ToRadians(lat1Degrees);
        var startLong = ToRadians(lon1Degrees);
        var endLat = ToRadians(lat2Degrees);
        var endLong = ToRadians(lon2Degrees);
        var dLong = endLong - startLong;
        var dPhi = DecimalEx.Log(DecimalEx.Tan(endLat / 2.0m + DecimalEx.Pi / 4.0m) /
                                 DecimalEx.Tan(startLat / 2.0m + DecimalEx.Pi / 4.0m));

        if (Abs(dLong) > DecimalEx.Pi) dLong = (2.0m * DecimalEx.Pi - dLong) * dLong > 0.0m ? -1 : 1;

        return ToDegrees(DecimalEx.ATan2(dLong, dPhi) + 360.0m) % 360;
    }
}