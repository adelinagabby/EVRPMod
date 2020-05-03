using System;
using System.Collections.Generic;

namespace EVRPMod
{
    public partial class CustomerData
    {
        public int Id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? KitType { get; set; }
        public int? Count { get; set; }
    }
}
