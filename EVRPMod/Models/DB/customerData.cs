using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class customerData
    {
        public int id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int? kitType { get; set; }
        public int? count { get; set; }
        public string address { get; set; }
        public int? orderAddress { get; set; }
        public int? depot { get; set; }
    }
}
