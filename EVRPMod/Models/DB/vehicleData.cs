using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class vehicleData
    {
        public int id { get; set; }
        public string name { get; set; }
        public float? capacity { get; set; }
        public float? serviceCost { get; set; }
        public float? costRoads { get; set; }
        public int? orderInAlgoritm { get; set; }
    }
}
