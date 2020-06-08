using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class AverageSpeedTable
    {
        public int id { get; set; }
        public int? rowTable { get; set; }
        public int? columnTable { get; set; }
        public float? valueTable { get; set; }
    }
}
