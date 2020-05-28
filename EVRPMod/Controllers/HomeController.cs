using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;


namespace EVRPMod.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult VehicleData()
        {
            ViewBag.Message = "Данные о ТС.";

            return View();
        }

        struct Coordinate
        {
            public string latitude, longitude;

        }

        struct CoordinateAndCountDepotAndCustomer
        {
            public int countDepot, countCustomer;
            public List<Coordinate> CoordinateDepot, CoordinateCustomer;

        }
        struct CoordinateAndCount
        {
            public int count;
            public List<Coordinate> Coordinate;
        }

        public struct StatesVariables
        {
            public bool ConsideringTypeAndQualityOfRoads, SeparateDeliveryAccounting;
          
        }

        public static StatesVariables StatesVariablesAlgoritm = new StatesVariables() {ConsideringTypeAndQualityOfRoads = false, SeparateDeliveryAccounting = false};


        [HttpPost]
        public ActionResult GetStatesVariables()
        {
            EVRPModContext db = new EVRPModContext();

            var AlgorithmSettings = db.AlgorithmSettings.ToList();
            StatesVariablesAlgoritm.ConsideringTypeAndQualityOfRoads = AlgorithmSettings.Where(x => x.variable == "ConsideringTypeAndQualityOfRoads").First().state;
            StatesVariablesAlgoritm.SeparateDeliveryAccounting = AlgorithmSettings.Where(x => x.variable == "SeparateDeliveryAccounting").First().state;

            return Json(StatesVariablesAlgoritm);
        }

        [HttpPost]
        public ActionResult SetStatesVariables(bool ConsideringTypeAndQualityOfRoads, bool SeparateDeliveryAccounting)
        {
            EVRPModContext db = new EVRPModContext();


            StatesVariablesAlgoritm.ConsideringTypeAndQualityOfRoads = ConsideringTypeAndQualityOfRoads;
            StatesVariablesAlgoritm.SeparateDeliveryAccounting = SeparateDeliveryAccounting;


            var AlgorithmSettings = db.AlgorithmSettings.ToList();

            db.AlgorithmSettings.Where(x => x.variable == "ConsideringTypeAndQualityOfRoads").First().state = ConsideringTypeAndQualityOfRoads;
            db.AlgorithmSettings.Where(x => x.variable == "SeparateDeliveryAccounting").First().state = SeparateDeliveryAccounting;
            //var ObjConsideringTypeAndQualityOfRoads = db.AlgorithmSettings.FirstOrDefault(x => x.variable == "ConsideringTypeAndQualityOfRoads");
            //var ObjSeparateDeliveryAccounting = db.AlgorithmSettings.FirstOrDefault(x => x.variable == "SeparateDeliveryAccounting");

            //ObjConsideringTypeAndQualityOfRoads.state = ConsideringTypeAndQualityOfRoads;
            //ObjSeparateDeliveryAccounting.state = SeparateDeliveryAccounting;


            //db.Entry(ObjConsideringTypeAndQualityOfRoads).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //db.Entry(ObjSeparateDeliveryAccounting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();

            return Json(StatesVariablesAlgoritm);
        }

        [HttpPost]
        public ActionResult FindingDistancesBetweenCustomersAndDepots()
        {

            AdditionalVariablesAndFunctions.ArrangementOfAddresses();

            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            List<Coordinate> CoordinateDepot = new List<Coordinate>();
            List<Coordinate> CoordinateCustomer = new List<Coordinate>();
            //for()
            //Coonew Coordinate
            //CoordinateAllAddress.Add(new Coordinate() {latitude = });

            foreach (var item in DepotData)
            {
                CoordinateDepot.Add(new Coordinate() { latitude = item.latitude, longitude = item.latitude });
            }
            foreach (var item in CustomerData)
            {
                CoordinateCustomer.Add(new Coordinate() { latitude = item.latitude, longitude = item.latitude });
            }

            CoordinateAndCountDepotAndCustomer CoordinateAndCount = new CoordinateAndCountDepotAndCustomer() { CoordinateDepot = CoordinateDepot, CoordinateCustomer = CoordinateCustomer, countDepot = DepotData.Count, countCustomer = CustomerData.Count };

            return Json(CoordinateAndCount);
        }



        [HttpPost]
        public ActionResult FindingMatrixsDistancesBetweenCustomersAndDepots(string[][] distanceMatrixBetweenCustomersAndDepots)
        {

            double[][] doubleDistanceMatrixBetweenCustomersAndDepots = new double[distanceMatrixBetweenCustomersAndDepots.Length][];

            for (int i = 0; i < distanceMatrixBetweenCustomersAndDepots.Length; i++)
            {
                doubleDistanceMatrixBetweenCustomersAndDepots[i] = new double[distanceMatrixBetweenCustomersAndDepots.Length];
                for (int j = 0; j < distanceMatrixBetweenCustomersAndDepots[0].Length; j++)
                {
                    doubleDistanceMatrixBetweenCustomersAndDepots[i][j] = Convert.ToDouble(distanceMatrixBetweenCustomersAndDepots[i][j].Replace(".", ","));
                }
            }

            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();
            var AverageSpeedData = db.AverageSpeedTable.ToList();
            var AverageRoadIntensityData = db.AverageRoadIntensityTable.ToList();
            var RoadQualityData = db.RoadQualityTable.ToList();
            var CostData = db.costTable.ToList();



            //Получение матриц дорог между депо и клиентами
            double[][] MatrixAverageSpeed = new double[distanceMatrixBetweenCustomersAndDepots.Length][];
            double[][] MatrixRoadQuality = new double[distanceMatrixBetweenCustomersAndDepots.Length][];
            double[][] MatrixAverageRoadIntensity = new double[distanceMatrixBetweenCustomersAndDepots.Length][];


            for (int i = 0; i < DepotData.Count; i++)
            {
                MatrixAverageSpeed[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
                MatrixRoadQuality[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
                MatrixAverageRoadIntensity[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];

                for (int j = 0; j < CustomerData.Count; j++)
                {
                    MatrixAverageSpeed[i][j] = AverageSpeedData.Where(x => x.rowTable == DepotData[i].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                    MatrixRoadQuality[i][j] = RoadQualityData.Where(x => x.rowTable == DepotData[i].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                    MatrixAverageRoadIntensity[i][j] = AverageRoadIntensityData.Where(x => x.rowTable == DepotData[i].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                }
            }


            doubleDistanceMatrixBetweenCustomersAndDepots = MethodKiniRayfa.ModificationOfMatrix(doubleDistanceMatrixBetweenCustomersAndDepots,
                MatrixAverageSpeed,MatrixRoadQuality,MatrixAverageRoadIntensity);

            DistributionOfCustomersByDepot(doubleDistanceMatrixBetweenCustomersAndDepots);

        


            List<CoordinateAndCount> MatrixCoordinateAllAddress = new List<CoordinateAndCount>();


            foreach (var itemDepot in DepotData)
            {
              //  List<CoordinateAndCount> CoordinateAndCount = new List<CoordinateAndCount>();
                List<Coordinate> CoordinateAllAddress = new List<Coordinate>();
                CoordinateAllAddress.Add(new Coordinate() { latitude = itemDepot.latitude, longitude = itemDepot.longitude });
                int orderInAlgoritm = 0;
                itemDepot.orderInAlgoritm = orderInAlgoritm;
                CustomerData = CustomerData.Where(x => x.depot == itemDepot.id).ToList();
                foreach (var itemCustomer in CustomerData)
                {
                    orderInAlgoritm++;
                    itemCustomer.orderInAlgoritm = orderInAlgoritm;
                    CoordinateAllAddress.Add(new Coordinate() { latitude = itemCustomer.latitude, longitude = itemCustomer.longitude });
                }
               // CoordinateAndCount.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
                MatrixCoordinateAllAddress.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
            }



            //?CoordinateAndCount
            return Json(MatrixCoordinateAllAddress);
        }

        [HttpPost]
        public ActionResult FindingShortestPaths(string[][][] distanceMatrixBetweenCustomersAndDepots)
        {


            double[][][] doubleDistanceMatrixBetweenCustomersAndDepots = new double[distanceMatrixBetweenCustomersAndDepots.Length][][];

            for (int k = 0; k < distanceMatrixBetweenCustomersAndDepots.Length; k++)
            {
                doubleDistanceMatrixBetweenCustomersAndDepots[k] = new double[distanceMatrixBetweenCustomersAndDepots.Length][];
                for (int i = 0; i < distanceMatrixBetweenCustomersAndDepots[k].Length; i++)
                {
                    doubleDistanceMatrixBetweenCustomersAndDepots[k][i] = new double[distanceMatrixBetweenCustomersAndDepots.Length];
                    for (int j = 0; j < distanceMatrixBetweenCustomersAndDepots[k].Length; j++)
                    {
                        doubleDistanceMatrixBetweenCustomersAndDepots[k][i][j] = Convert.ToDouble(distanceMatrixBetweenCustomersAndDepots[k][i][j].Replace(".", ","));
                    }
                }
            }


         
            return Json(0);
        }

        public void DistributionOfCustomersByDepot(double[][] distanceFromOrdersToDepot)
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            for (int i = 0; i < CustomerData.Count; i++)
            {
                double min = 999999999;
                int minDepot = -1;
                for (int j = 0; j < DepotData.Count; j++)
                {
                    if (min > distanceFromOrdersToDepot[j][i])
                    {
                        min = distanceFromOrdersToDepot[j][i];
                        minDepot = j;
                    }
                }

                CustomerData[i].depot = DepotData[minDepot].id;
            }
            db.SaveChanges();
        }
        public ActionResult ModificationDistancesBetweenCustomersAndDepots(double[][] distanceFromOrdersToDepot, double[][] MatrixOfEstimatesOfPermittedVelocities,
             double[][] MatrixOfQualityOfRoads, double[][] MatrixOfNumbersOfLights)
        {
            distanceFromOrdersToDepot = MethodKiniRayfa.ModificationOfMatrix(distanceFromOrdersToDepot, MatrixOfEstimatesOfPermittedVelocities, MatrixOfQualityOfRoads, MatrixOfNumbersOfLights);
            return Json(distanceFromOrdersToDepot);
        }

    }
    }