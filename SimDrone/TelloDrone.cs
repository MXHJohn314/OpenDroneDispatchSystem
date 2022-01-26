using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using SimDrone.Controllers;

namespace SimDrone;

public class TelloDrone : Drone
{
    // Converted by hand from python code originally from Tello DJI
    // Adapter between FlyingPizza dispatcher and Tello SDK Drone
    private const decimal LongToCm = 1110000;
    private const string Command = "command";
    private const string Right = "right ";
    private const string Left = "left ";
    private const string Backward = "back ";
    private const string Forward = "forward ";
    private const string Takeoff = "takeoff";
    private const string Land = "land";
    private const decimal MaxDist = 200;
    private const decimal MinDist = 20;
    private string? _direction;
    private readonly IPAddress _telloAddress = IPAddress.Parse("192.168.10.1");
    private readonly int _telloPort = 8889;
    private readonly UdpClient? _socket;
    private readonly int _battery;
    private double _speed;
    private int _altitude;
    private new static string IpAddress = "192.168.10.1";
    private readonly DateTimeOffset _elapsed;
    private GeoLocation? CalcDestination { get; set; }
    private GeoLocation? CalcDestinationCm { get; set; }
    private bool Offline { get; set; }

    public List<GeoLocation> Route { get; set; }

    private static decimal CmToArc(decimal value)
    {
        // Converts tello centimeter distance to lat/long.
        return value / LongToCm;
    }

    public TelloDrone(DroneRecord record,IDroneToDispatchGateway gateway, SimDroneController controller, bool offline = true) : base(record,gateway,controller)
    {
        Route = new List<GeoLocation>();
        Id = record.Id;
        State = DroneState.Ready;
        CurrentLocation = record.HomeLocation;
        Destination = record.Destination;
        HomeLocation = record.HomeLocation;
        Offline = offline;
        if (offline)
        {
            _battery = 100;
            _speed = 0.0;
            _elapsed = DateTimeOffset.Now;
            _altitude = 0;
        }
        else
        {
            _battery = 0;
            _speed = 0.0;
            _elapsed = DateTimeOffset.Now;
            _altitude = 0;
            _socket = new UdpClient(_telloPort);
            _socket.Connect(_telloAddress, _telloPort);
        }

    }

    public new GeoLocation[] GetRoute()
    {
        Route = new List<GeoLocation> {CurrentLocation};
        PointToTelloCommands(Destination.Latitude, Destination.Longitude);
        PointToTelloCommands(HomeLocation.Latitude, HomeLocation.Longitude);
        return Route.ToArray();

    }

    public new async Task<AssignDeliveryResponse> DeliverOrder(AssignDeliveryRequest request)
    {
        var successful = DeliverOrder(request.OrderLocation);
        return new AssignDeliveryResponse
        {
            DroneId = request.DroneId,
            OrderId = request.OrderId,
            Success = successful
        };
    }

    public bool DeliverOrder(GeoLocation customerLocation)
    {
        Destination = customerLocation;
        State = DroneState.Delivering;
        SendCommand(Command);
        SendCommand(Takeoff);
        SendCommand(customerLocation);
        SendCommand(Land);
        SendCommand(Takeoff);
        State = DroneState.Returning;
        SendCommand(HomeLocation);
        SendCommand(Land);
        State = DroneState.Ready;
        return true;
    }

    private void SendCommand(GeoLocation telemetry)
    {
        var commands = PointToTelloCommands(telemetry.Latitude, telemetry.Longitude);
        if (State == DroneState.Dead) return;
        foreach (string command in commands)
        {
            var task = send_command(command, Offline);
            task.Wait();
        }
    }

    private void SendCommand(string command)
    {
        if (State == DroneState.Dead) return;
        // Refuses to take commands if an error occured.
        var task = send_command(command, Offline);
        task.Wait();
        if (!task.Result)
        {
            // Errors are considered in dead status for now
            State = DroneState.Dead;
        }
    }

    private static decimal ArcToCm(decimal value)
    {
        // Converts lat/long to tello centimeter distance.
        return value * LongToCm;
    }

    private static bool Approximate(decimal valueA, decimal valueB)
    {
        return valueA > (valueB - MinDist) && valueA < (valueB + MinDist);
    }

    private IEnumerable<string> PointToTelloCommands(decimal lat, decimal longitude)
    {
        // Unrolls a lat/long command into multiple Tello 200cm directions
        CalcDestination = new GeoLocation
        {
            Latitude = lat,
            Longitude = longitude
        };
        CalcDestinationCm = new GeoLocation
        {
            Latitude = ArcToCm(CalcDestination.Latitude),
            Longitude = ArcToCm(CalcDestination.Longitude)
        };
        decimal tempX = ArcToCm(CurrentLocation.Latitude);
        decimal tempY = ArcToCm(CurrentLocation.Longitude);
        decimal amount = 0;
        //double telloFudgeValue = 0.45;
        // This value was added since I observed a command of 111 cm ends up being about 50 cm in distance
        // You may have to calibrate your tello for this factor to be accurate.
        var commands = new List<string>();
        while (!(tempX == CalcDestinationCm.Latitude && tempY == CalcDestinationCm.Longitude))
        {
            if(Approximate(tempX, CalcDestinationCm.Latitude) && Approximate(tempY, CalcDestinationCm.Longitude))
            {
                // Equipment too granular, 20cm circular range is close enough, and drone lands
                tempX = CalcDestinationCm.Latitude;
                tempY = CalcDestinationCm.Longitude;
            }
            string tempCommand;
            while (tempY < CalcDestinationCm.Longitude - MinDist)
            {
                _direction = Forward;
                var diffY = Math.Abs(CalcDestinationCm.Longitude - tempY);
                if (diffY >= MaxDist)
                {
                    amount = MaxDist;

                }

                if (diffY < MaxDist)
                {
                    amount = Math.Abs(tempY - CalcDestinationCm.Longitude);

                }

                tempY += amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
                Route.Add(new GeoLocation {Latitude = CmToArc(tempX), Longitude = CmToArc(tempY)});
                amount = 0;
            }

            amount = 0;
            while (tempX > CalcDestinationCm.Latitude + MinDist)
            {

                _direction = Left;
                var diffX = Math.Abs(tempX - CalcDestinationCm.Latitude);
                if (diffX >= MaxDist)
                {
                    amount = MaxDist;

                }

                if (diffX < MaxDist)
                {
                    amount = Math.Abs(tempX - CalcDestinationCm.Latitude);

                }

                tempX -= amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
                Route.Add(new GeoLocation {Latitude = CmToArc(tempX), Longitude = CmToArc(tempY)});
                amount = 0;
            }

            amount = 0;
            while (tempX < CalcDestinationCm.Latitude - MinDist)
            {

                _direction = Right;
                var diffX = Math.Abs(tempX - CalcDestinationCm.Latitude);
                if (diffX >= MaxDist)
                {
                    amount = MaxDist;

                }

                if (diffX < MaxDist)
                {
                    amount = Math.Abs(tempX - CalcDestinationCm.Latitude);

                }

                tempX += amount;

                tempCommand = _direction + amount;
                commands.Add(tempCommand);
                Route.Add(new GeoLocation {Latitude = CmToArc(tempX), Longitude = CmToArc(tempY)});
                amount = 0;
            }



            amount = 0;
            while (tempY > CalcDestinationCm.Longitude + MinDist)
            {
                _direction = Backward;
                var diffY = Math.Abs(CalcDestinationCm.Longitude - tempY);
                if (diffY >= MaxDist)
                {
                    amount = MaxDist;
                }

                if (diffY < MaxDist)
                {
                    amount = Math.Abs(tempY - CalcDestinationCm.Longitude);

                }

                tempY -= amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
                Route.Add(new GeoLocation {Latitude = CmToArc(tempX), Longitude = CmToArc(tempY)});
                amount = 0;
            }

            amount = 0;
            // For now we update the location after unrolling since Tello doesn't keep lat/long.
            CurrentLocation = new GeoLocation
            {
                Latitude = CmToArc(tempX),
                Longitude = CmToArc(tempY)
            };
        }
        Console.WriteLine(string.Join("\n", commands));
        return commands.ToArray();
    }

    public override string ToString()
    {
        return $"location:{CurrentLocation}\nDestination:{Destination}\nStatus:{State}";
    }
    
   
    public async Task<bool> send_command(string command, bool offline = false)
    {
        if (offline)
        {
            var splitCommand = command.Split(" ");
            State = DroneState.Dead;
            if (splitCommand.Length >= 2 && splitCommand[1].Length > 0)
            {
                switch (splitCommand[0])
                {

                    case "up":
                        _altitude += int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "down":
                        _altitude -= int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "left":
                        CurrentLocation.Latitude -= CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "right":
                        CurrentLocation.Latitude += CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "forward":
                        CurrentLocation.Longitude += CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "back":
                        CurrentLocation.Longitude -= CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "mdirection":
                        return await Task.FromResult(true);
                    case "speed":
                        _speed = int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "mon":
                        return await Task.FromResult(true);
                    case "stop":
                        _speed = 0;
                        return await Task.FromResult(true);
                    case "cw":
                        return await Task.FromResult(true);
                    case "ccw":
                        return await Task.FromResult(true);
                    case "flip":
                        return await Task.FromResult(true);
                }
            }
            else
            {
                switch (splitCommand[0])
                {

                    case "command":
                        return await Task.FromResult(true);
                    case "takeoff":
                        return await Task.FromResult(true);
                    case "land":
                        return await Task.FromResult(true);
                    case "speed?":
                        return await Task.FromResult(true);
                    case "battery?":
                        return await Task.FromResult(true);
                    case "time?":
                        return await Task.FromResult(true);
                    case "wifi?":
                        return await Task.FromResult(true);
                    case "sdk?":
                        return await Task.FromResult(true);
                    case "sn?":
                        return await Task.FromResult(true);
                    case "stop":
                        return await Task.FromResult(true);
                    case "emergency":
                        return await Task.FromResult(true);
                }
            }

            return await Task.FromResult(false);
        }

        byte[] bytes = Encoding.UTF8.GetBytes(command);

        if (command == "takeoff")
        {
            // Added since takeoff ignores settling on the floor
            await Task.Delay(5000);
        }

        await _socket?.SendAsync(bytes, bytes.Length)!;
        var response = await _socket.ReceiveAsync();
        var telloResp = Encoding.ASCII.GetChars(response.Buffer);
        var responseString = new string(telloResp);
        Console.WriteLine(responseString);
        return responseString.Contains("ok");
        // If ok is not sent, an error or invalid command occurs, shuts down automatically
    }

}