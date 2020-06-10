using System;
using System.Collections.Generic;

namespace EVRPMod.Models.DB
{
    public partial class kitType
    {
        public int id { get; set; }
        public string name { get; set; }
        public float? weight { get; set; }
    }
}
