using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class vehicleInDepot
    {
        public int id { get; set; }
        public int? depotId { get; set; }
        public int? vehicleId { get; set; }
        public int? count { get; set; }
    }
}
