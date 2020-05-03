using System;
using System.Collections.Generic;

namespace EVRPMod
{
    public partial class VehicleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Capacity { get; set; }
        public int? Depot { get; set; }
        public int? ServiceCost { get; set; }
        public int? CostRoads { get; set; }
    }
}
