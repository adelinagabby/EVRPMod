using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EVRPMod.Models.DB;
using Microsoft.Ajax.Utilities;

namespace EVRPMod
{
    public class AdditionalVariablesAndFunctions
    {
        //таблицы учета дорог сохранены
        public static bool RoadAccountingTablesAreSaved = false;
        //таблицы учета дорог заполнены
        public static bool RoadTablesAreFilled = false;
        //учет дорог
        public static bool RoadAccounting = true;


        public static void ArrangementOfAddresses()
        {
            EVRPModContext db = new EVRPModContext();


            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            //обнуляем порядок
            for (int i = 0; i < CustomerData.Count; i++)
            {
                CustomerData[i].orderAddress = -1;
            }
            for (int i = 0; i < DepotData.Count; i++)
            {
                DepotData[i].orderAddress = -1;
            }


            int orderAddress = 0;
            //устанавливаем порядок
            for (int i = 0; i < CustomerData.Count; i++)
            {
                if(CustomerData[i].orderAddress == -1)
                {
                    //CustomerData[i].orderAddress = orderAddress;
                    CustomerData.Where(x => x.latitude == CustomerData[i].latitude && x.longitude == CustomerData[i].longitude).ForEach(x=>x.orderAddress = orderAddress);
                    orderAddress++;
                }
            }
            for (int i = 0; i < DepotData.Count; i++)
            {
                DepotData[i].orderAddress = orderAddress;
                orderAddress++;
            }

            db.SaveChanges();
        }



        //public static void VehicleOrderInAlgorithm()
        //{

        //    EVRPModContext db = new EVRPModContext();

        //    var VehicleData = db.vehicleData.ToList();
        //    var DepotData = db.depotData.OrderBy(x => x.orderAddress).ToList();

        //    for (int i = 0; i < DepotData.Count; i++)
        //    {
        //        int orderInAlgoritm = 0;
        //        for (int j = 0; j < VehicleData.Count; j++)
        //        {
        //            if(VehicleData[j].)
        //            VehicleData[i].orderInAlgoritm = i;
        //        }
        //    }
          
        //    db.SaveChanges();
        //}
    }
}