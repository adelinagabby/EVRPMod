using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class vehicleData
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? capacity { get; set; }
        public int? serviceCost { get; set; }
        public int? costRoads { get; set; }
    }
}
