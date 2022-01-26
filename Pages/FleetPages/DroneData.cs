using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyingDrone.Pages.FleetPages
{
    public class DroneData
    {

        public string DroneID { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        public string Order { get; set; }
        public string batteryLife { get; set; }
        public Point location { get;set;}
        public Point destination { get; set; }

    }
}
