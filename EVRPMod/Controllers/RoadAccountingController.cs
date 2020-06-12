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
            int orderAddress = -1;
            foreach (var itemCustomerData in CustomerData)
            {
                if (itemCustomerData.orderAddress != orderAddress)
                {
                    Address.Add(itemCustomerData.address);
                    //orderAddress = (int) itemCustomerData.orderAddress;
                }
            }
            foreach (var itemDepotData in DepotData)
            {
                Address.Add(itemDepotData.address);
            }

            return Json(Address);
        }


        struct RoadTable
        {
            public double[][] AverageSpeedTable;
            public double[][] AverageRoadIntensityTable;
            public double[][] RoadQualityTable;
            public double[][] CostTable;
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
                double[][] AverageSpeedTable = new double[countAddress][];
                double[][] AverageRoadIntensityTable = new double[countAddress][];
                double[][] RoadQualityTable = new double[countAddress][];
                double[][] CostTable = new double[countAddress][];

                for (int i = 0; i < countAddress; i++)
                {
                    AverageSpeedTable[i] = new double[countAddress];
                    AverageRoadIntensityTable[i] = new double[countAddress];
                    RoadQualityTable[i] = new double[countAddress];
                    CostTable[i] = new double[countAddress];
                    for (int j = 0; j < countAddress; j++)
                    {
                        AverageSpeedTable[i][j] = Math.Round(AverageSpeedData.Where(x => x.rowTable == i && x.columnTable == j).FirstOrDefault()?.valueTable ?? 0);
                        AverageRoadIntensityTable[i][j] = Math.Round(AverageRoadIntensityData.Where(x => x.rowTable == i && x.columnTable == j).FirstOrDefault()?.valueTable ?? 0);
                        RoadQualityTable[i][j] = Math.Round(RoadQualityData.Where(x => x.rowTable == i && x.columnTable == j).FirstOrDefault()?.valueTable ?? 0);
                        CostTable[i][j] = Math.Round(CostData.Where(x => x.rowTable == i && x.columnTable == j).FirstOrDefault()?.valueTable ?? 0);
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


        public ActionResult SaveTables(string[][] AverageSpeedTable, string[][] AverageRoadIntensityTable, string[][] RoadQualityTable, string[][] CostTable)
        {
            //AdditionalVariablesAndFunctions.ArrangementOfAddresses();

            //AdditionalVariablesAndFunctions.RoadAccountingTablesAreSaved = true;
         
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
            float valueTableAverageSpeedTable;
            float valueTableAverageRoadIntensityTable;
            float valueTableRoadQualityTable;
            float valueTableCostTable;
            //float value = 0.000f;
            try
            {
                for (int i = 0; i < countTable; i++)
                {
                    for (int j = 0; j < countTable; j++)
                    {

                        valueTableAverageSpeedTable = string.IsNullOrEmpty(AverageSpeedTable[i][j]) ? 0 : float.Parse(AverageSpeedTable[i][j].Replace(".", ","));
                        valueTableAverageRoadIntensityTable = string.IsNullOrEmpty(AverageRoadIntensityTable[i][j]) ? 0 : float.Parse(AverageRoadIntensityTable[i][j].Replace(".", ","));
                        valueTableRoadQualityTable = string.IsNullOrEmpty(RoadQualityTable[i][j]) ? 0 : float.Parse(RoadQualityTable[i][j].Replace(".", ","));
                        valueTableCostTable= string.IsNullOrEmpty(CostTable[i][j]) ? 0 : float.Parse(CostTable[i][j].Replace(".", ","));
                        if (valueTableAverageSpeedTable<20 && valueTableAverageSpeedTable != 0 || valueTableAverageSpeedTable > 110 && valueTableAverageSpeedTable != 0)
                            return Json("Ошибка. Значения средней скорости выходят за пределы");
                        else if (valueTableRoadQualityTable < 1 && valueTableRoadQualityTable != 0 || valueTableRoadQualityTable > 10 && valueTableRoadQualityTable != 0)
                            return Json("Ошибка. Значения среднего качества дорог выходят за пределы");
                        else if (valueTableAverageRoadIntensityTable < 200 && valueTableAverageRoadIntensityTable != 0 || valueTableAverageRoadIntensityTable > 14000 && valueTableAverageRoadIntensityTable != 0)
                            return Json("Ошибка. Значения интенсивности движения выходят за пределы");
                        else if (valueTableCostTable < 0)
                            return Json("Ошибка. Значения протяженности платных дорог не должны быть ниже нуля");

                        var newObjAverageSpeedTable = new AverageSpeedTable
                        {
                            id = id,
                            rowTable = i,
                            columnTable = j,
                            valueTable = valueTableAverageSpeedTable

                            //valueTable = float.TryParse(AverageSpeedTable[i][j].Replace(".", ","), out value) ? 0 : float.Parse(AverageSpeedTable[i][j].Replace(".", ","))
                            //valueTable = Convert.ToDouble(string.IsNullOrEmpty(AverageSpeedTable[i][j]) ? "0" : (double) AverageSpeedTable[i][j])

                        };
                        var newObjAverageRoadIntensityTable = new AverageRoadIntensityTable
                        {
                            id = id,
                            rowTable = i,
                            columnTable = j,
                            valueTable = valueTableAverageRoadIntensityTable
                            //valueTable = Convert.ToInt32(string.IsNullOrEmpty(AverageRoadIntensityTable[i][j]) ? "0" : AverageRoadIntensityTable[i][j])

                        };
                        var newObjRoadQualityTable = new RoadQualityTable
                        {
                            id = id,
                            rowTable = i,
                            columnTable = j,
                            valueTable = valueTableRoadQualityTable
                            //valueTable = Convert.ToInt32(string.IsNullOrEmpty(RoadQualityTable[i][j]) ? "0" : RoadQualityTable[i][j])

                        };
                        var newObjCostTable = new costTable
                        {
                            id = id,
                            rowTable = i,
                            columnTable = j,
                            valueTable = valueTableCostTable
                            //valueTable = Convert.ToInt32(string.IsNullOrEmpty(CostTable[i][j]) ? "0" : CostTable[i][j])

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
                db.AlgorithmSettings.Where(x => x.variable == "RoadAccountingTablesAreSaved").FirstOrDefault().state = true;
             
                db.SaveChanges();
                return Json(0);
            }
            catch
            {
                return Json("Ошибка. Введено нечисловое значение");
            }

          
        }
    }
}