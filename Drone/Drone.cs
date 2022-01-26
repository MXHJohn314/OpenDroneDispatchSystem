using System;
using System.Threading;
using System.Collections.Generic;

namespace FlyingDrone
{
    public enum DroneState
    {
        READY,
        DELIVERING,
        RETURNING,
        DEAD,
        CHARGING
    }

    public readonly record struct Point(double X , double Y);
    public class Drone
    {
        // The unique ID of this drone which is stored in the database
        private int Id { get; }

        // The point representing the pizza restaurant
        private Point Home { get; }

        // The current position of the drone
        private Point Location { get; set; }

        // The desired position of the drone
        public Point Destination { get; set; }

        // Current status of the drone
        private DroneState Status { get; set; }
        
        // Constructor
        public Drone(int id, Point Home)
        {
            this.Id = id;
            Location = Home = Destination = Home;
            Status = DroneState.READY;
        }

        // Calculate the next Point.X or Point.Y along a route
        double routeStep(double v, int i, int numberOfLocations, bool isLatitude) 
            => isLatitude && Home.X == Destination.X
                ? Home.X
            : !isLatitude && Home.Y == Destination.Y
                ? Home.Y
            : (v * (i + 1)) / numberOfLocations;

        // Return an array of Point records simulating a drone's delivery route
        public Point[] GetRoute()
        {
            if (Home.X == Destination.X && Home.Y == Destination.Y)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            double xDistance = Home.X - Destination.X;

            // Latitude distance to get to destination
            double yDistance = Home.Y - Destination.Y;

            // # of locations should be the absolute value of the hypotenuse, rounded up to the
            // nearest integer
            int numberOfLocations = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));

            // LINQ yields all the points (except possibly the last one) along the route, one unit apart.
            List<Point> route = Enumerable.Range(0, numberOfLocations - 1)
                .Select(i => new Point(
                    routeStep(xDistance, i, numberOfLocations, true),
                    routeStep(yDistance, i, numberOfLocations, false)
                )).ToList();

            // Add the Destination Point if needed.
            if (!route.TakeLast(1).Equals(Destination))
            {
                route.Add(Destination);
            }
            return route.ToArray();
        }

        // Dispatch a drone to deliver a pizza.
        public void deliverOrder(Point customerLocation)
        {
            Destination = customerLocation;
            UpdateStatus(DroneState.DELIVERING);
            Point[] route = GetRoute();
            Console.WriteLine(this);

            for (int i = 0; i < route.Length; i++)
            {
                Location = route[i];
                UpdateLocation(Location);
                Console.WriteLine(this);
                Thread.Sleep(2000);
            }

            UpdateStatus(DroneState.RETURNING);
            for (int i = route.Length - 1; i >0; i--)
            {
                Location = route[i];
                UpdateLocation(Location);
                Console.WriteLine(this);
                Thread.Sleep(2000);
            }
            UpdateStatus(DroneState.READY);
        }

        // Todo: Post a status update to the database.
        private void UpdateStatus(DroneState state)
        {
            this.Status = state;
            throw new NotImplementedException();
        }

        // Todo: Post a location update to the database. 
        private void UpdateLocation(Point location)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"ID:{Id}\nlocation:{Location}\nDestination:{Destination}\nStatus:{Status}";
        }
    }
}
