using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EVRPMod.Models.DB;



namespace EVRPMod
{
    public class AdditionalVariablesAndFunctions
    {

        public static void ArrangementOfAddresses()
        {
            EVRPModContext db = new EVRPModContext();


            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();


            for (int i = 0; i < CustomerData.Count; i++)
            {
                CustomerData[i].orderAddress = i;
            }
            for (int i = 0; i < DepotData.Count; i++)
            {
                DepotData[i].orderAddress = i + CustomerData.Count;
            }

        }

    }
}