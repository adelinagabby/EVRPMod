using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class RoadAccountingController : Controller
    {
        // GET: RoadAccounting
        public ActionResult RoadAccounting()
        {
            return View();
        }
        public ActionResult GetAddressList()
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.OrderBy(x=>x.orderAddress).ToList();
            var CustomerData = db.customerData.OrderBy(x => x.orderAddress).ToList();

            List<string> Address = new List<string>();

            foreach (var itemCustomerData in CustomerData)
            {
                Address.Add(itemCustomerData.address);
            }
            foreach (var itemDepotData in DepotData)
            {
                Address.Add(itemDepotData.address);
            }

            return Json(Address);
        }


        struct RoadTable
        {
            public int[,] AverageSpeedTable;
            public int[,] AverageRoadIntensityTable;
            public int[,] RoadQualityTable;
            public int[,] CostTable;
        }

        public ActionResult GetTables()
        {
            EVRPModContext db = new EVRPModContext();

            var AverageSpeedData = db.AverageSpeedTable.ToList();
            var AverageRoadIntensityData = db.AverageRoadIntensityTable.ToList();
            var RoadQualityData = db.RoadQualityTable.ToList();
            var CostData = db.costTable.ToList();

            if (AverageSpeedData.Any() && AverageRoadIntensityData.Any() && RoadQualityData.Any() && CostData.Any())
            {
                int countAddress = (int)Math.Sqrt(AverageSpeedData.Count);
                int[,] AverageSpeedTable = new int[countAddress, countAddress];
                int[,] AverageRoadIntensityTable = new int[countAddress, countAddress];
                int[,] RoadQualityTable = new int[countAddress, countAddress];
                int[,] CostTable = new int[countAddress, countAddress];

                for (int i = 0; i < countAddress; i++)
                {
                    for (int j = 0; j < countAddress; j++)
                    {
                        AverageSpeedTable[i, j] = AverageSpeedData.Where(x => x.rowTable == i && x.columnTable == j).First().valueTable ?? 0;
                        AverageRoadIntensityTable[i, j] = AverageRoadIntensityData.Where(x => x.rowTable == i && x.columnTable == j).First().valueTable ?? 0;
                        RoadQualityTable[i, j] = RoadQualityData.Where(x => x.rowTable == i && x.columnTable == j).First().valueTable ?? 0;
                        CostTable[i, j] = CostData.Where(x => x.rowTable == i && x.columnTable == j).First().valueTable ?? 0;
                    }
                }

                RoadTable RoadTable = new RoadTable() { AverageSpeedTable = AverageSpeedTable, AverageRoadIntensityTable = AverageRoadIntensityTable, 
                    RoadQualityTable = RoadQualityTable, CostTable = CostTable};

                //var DepotData = db.depotData.OrderBy(x => x.orderAddress).ToList();
                //var CustomerData = db.customerData.OrderBy(x => x.orderAddress).ToList();

                //List<string> Address = new List<string>();

                //foreach (var itemCustomerData in CustomerData)
                //{
                //    Address.Add(itemCustomerData.address);
                //}
                //foreach (var itemDepotData in DepotData)
                //{
                //    Address.Add(itemDepotData.address);
                //}

                return Json(RoadTable);
            }
            return Json(0);
        }


        public ActionResult SaveTables(int[,] AverageSpeedTable, int[,] AverageRoadIntensityTable, int[,] RoadQualityTable, int[,] CostTable)
        {
            //AdditionalVariablesAndFunctions.ArrangementOfAddresses();

            AdditionalVariablesAndFunctions.RoadAccountingTablesAreSaved = true;

            EVRPModContext db = new EVRPModContext();


            var AverageSpeedData = db.AverageSpeedTable.ToList();
            var AverageRoadIntensityData = db.AverageRoadIntensityTable.ToList();
            var RoadQualityData = db.RoadQualityTable.ToList();
            var CostData = db.costTable.ToList();


            db.AverageSpeedTable.RemoveRange(db.AverageSpeedTable.ToList());
            db.AverageRoadIntensityTable.RemoveRange(db.AverageRoadIntensityTable.ToList());
            db.RoadQualityTable.RemoveRange(db.RoadQualityTable.ToList());
            db.costTable.RemoveRange(db.costTable.ToList());

            int countTable = AverageSpeedTable.Length;
            int id = 0;
            for (int i = 0; i < countTable; i++)
            {
                for (int j = 0; j < countTable; j++)
                {
                    var newObjAverageSpeedTable = new AverageSpeedTable
                    {
                        id = id,
                        rowTable = i,
                        columnTable = j,
                        valueTable = AverageSpeedTable[i,j]

                    };
                    var newObjAverageRoadIntensityTable = new AverageRoadIntensityTable
                    {
                        id = id,
                        rowTable = i,
                        columnTable = j,
                        valueTable = AverageRoadIntensityTable[i, j]

                    };
                    var newObjRoadQualityTable = new RoadQualityTable
                    {
                        id = id,
                        rowTable = i,
                        columnTable = j,
                        valueTable = RoadQualityTable[i, j]

                    };
                    var newObjCostTable = new costTable
                    {
                        id = id,
                        rowTable = i,
                        columnTable = j,
                        valueTable = CostTable[i, j]

                    };
                    db.AverageSpeedTable.Add(newObjAverageSpeedTable);
                    db.AverageRoadIntensityTable.Add(newObjAverageRoadIntensityTable);
                    db.RoadQualityTable.Add(newObjRoadQualityTable);
                    db.costTable.Add(newObjCostTable);

                    id++;
                }
            }

            //var newObj = new depotData
            //{
            //    //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
            //    name = name,
            //    latitude = latitude,
            //    longitude = longitude,
            //    address = address,
            //};


            //db.depotData.Add(newObj);

            db.SaveChanges();



            return Json(0);
        }
    }
}