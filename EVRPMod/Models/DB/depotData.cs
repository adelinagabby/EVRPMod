using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class depotData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
    }
}
