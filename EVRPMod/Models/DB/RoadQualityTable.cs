﻿using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class RoadQualityTable
    {
        public int id { get; set; }
        public int? rowTable { get; set; }
        public int? columnTable { get; set; }
        public int? valueTable { get; set; }
    }
}