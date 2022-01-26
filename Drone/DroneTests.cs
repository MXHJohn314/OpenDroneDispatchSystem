using System;
using Xunit;

namespace FlyingDrone
{
    public class DroneTests
    {
        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var home = new Point(0.0, 0.0);
            var dest = new Point(3.0, 4.0);

            var drone = new Drone(1, home);
            drone.Destination = dest;

            Point[] expectedRoute = {
                new (0.6, 0.8),
                new (1.2, 1.6),
                new (1.8, 2.4),
                new (2.4, 3.2),
                new (3.0, 4.0)
            };
            Assert.Equal(expectedRoute, drone.GetRoute());
        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            var home = new Point(0.0, 0.0);
            var dest = new Point(-3.0, -4.0);

            var drone = new Drone(1, home);
            drone.Destination = dest;

            Point[] expectedRoute = {
                new (-0.6, -0.8),
                new (-1.2, -1.6),
                new (-1.8, -2.4),
                new (-2.4, -3.2),
                new (-3.0, -4.0)
            };
            Assert.Equal(expectedRoute, drone.GetRoute());
        }
    }
}
