using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;
using Microsoft.Ajax.Utilities;

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

        public struct CustomerAndKit
        {
            int Customer;
            int Kit;
            int Count;
        }

        public struct VehicleOrders
        {
            int Vehicle;
            List<CustomerAndKit> customersAndKits;
        }

        public struct Vehicle
        {
            public int VehicleId;
            public int carryingСapacity;
            public int loadOccupied;
            public int orderInAlgoritm;
        }

        public struct VehiclesOfDepot
        {
            int depot;
            List<VehicleOrders> vehicleOrders;
        }

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

            var DepotData = db.depotData.OrderBy(x=>x.orderAddress).ToList();
            var CustomerData = db.customerData.OrderBy(x => x.orderAddress).ToList();
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
                //List<CoordinateAndCount> CoordinateAndCount = new List<CoordinateAndCount>();
                List<Coordinate> CoordinateAllAddress = new List<Coordinate>();
                CoordinateAllAddress.Add(new Coordinate() { latitude = itemDepot.latitude, longitude = itemDepot.longitude });
                int orderInAlgoritm = 0;
                int orderAddress = -1;
                itemDepot.orderInAlgoritm = orderInAlgoritm;
                CustomerData = CustomerData.Where(x => x.depot == itemDepot.id).OrderBy(x=>x.orderAddress).ToList();
                foreach (var itemCustomer in CustomerData)
                {
                    
                    if (itemCustomer.orderAddress != orderAddress)
                    {
                        orderInAlgoritm++;
                        CustomerData.Where(x => x.orderAddress == itemCustomer.orderAddress).ForEach(x => x.orderInAlgoritm = orderInAlgoritm);
                       // itemCustomer.orderInAlgoritm = orderInAlgoritm;
                        orderAddress = (int) itemCustomer.orderAddress;
                        CoordinateAllAddress.Add(new Coordinate() { latitude = itemCustomer.latitude, longitude = itemCustomer.longitude });
                    }
                }
                //CoordinateAndCount.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
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


        public void FindingShortestPaths(double[][] distanceMatrixBetweenCustomersAndDepots, int depotId)
        {

            EVRPModContext db = new EVRPModContext();
            var VehicleData = db.vehicleData.OrderBy(x => x.orderInAlgoritm).ToList();
            var VehicleInDepot = db.vehicleInDepot.Where(x=>x.depotId == depotId).ToList();
            List<Vehicle> vehicles = new List<Vehicle>();
            //int Vehicle;
            //int carryingСapacity;
            //int loadOccupied;
            int orderVehicleInAlgoritm = 0;
            for (int i = 0; i < VehicleInDepot.Count; i++)
            {
                var objVehicle = VehicleData.Where(x=>x.id== VehicleInDepot[i].vehicleId).First();

                for (int j = 0; j < VehicleInDepot[i].count; j++)
                {
                   // Vehicle tempVehicle = new Vehicle { VehicleId = objVehicle.id, carryingСapacity = (int)objVehicle.capacity, orderInAlgoritm  = orderInAlgoritm};
                    vehicles.Add(new Vehicle { VehicleId = objVehicle.id, carryingСapacity = (int)objVehicle.capacity, orderInAlgoritm = orderVehicleInAlgoritm });
                    orderVehicleInAlgoritm++;
                }
            }

            List<VehicleOrders> bestVehicleOrdering = new List<VehicleOrders>();
            List<VehicleOrders> tempVehicleOrdering = new List<VehicleOrders>();

            int numberOfAddress = distanceMatrixBetweenCustomersAndDepots.Length;
            int numberOfVehicles = vehicles.Count;

            int numberIterationForVehicle = 1000;
            int numberIterationForOrder = 1000;

            //int[] bestWay = new int[matrixWay.Length + 1];
            //double bestCostWay = infinity;


            //List<int[]> populationForAddress = new List<int[]>();
            //List<int[]> newPopulationForAddress = new List<int[]>();


            List<int[]> populationForVehicle = new List<int[]>();
            List<int[]> newPopulationForVehicle = new List<int[]>();

            List<int[]> populationForOrder = new List<int[]>();
            List<int[]> newPopulationForOrder = new List<int[]>();

            int sizePopulationForVehicle = 10;
            int sizePopulationForOrder = 10;

            populationForVehicle = GeneticAlgoritm.PrimaryPopulation(sizePopulationForVehicle, numberOfVehicles);

            int[] tmpForVehicle;
            for (int i = 0; i < sizePopulationForVehicle; i++)//по наборам
            {
                tmpForVehicle = GeneticAlgoritm.GetSet(populationForVehicle, i);
                newPopulationForVehicle.Add(GeneticAlgoritm.GetSet(populationForVehicle, i));
            }


            while (numberIterationForVehicle > 0)
            {
                populationForOrder = GeneticAlgoritm.PrimaryPopulation(sizePopulationForOrder, numberOfVehicles);

                int[] tmpForOrder;
                for (int i = 0; i < sizePopulationForOrder; i++)//по наборам
                {
                    tmpForOrder = GeneticAlgoritm.GetSet(populationForOrder, i);
                    newPopulationForOrder.Add(GeneticAlgoritm.GetSet(populationForOrder, i));
                }

                //newPopulationForVehicle = GeneticAlgoritm.CreatingANewPopulation(newPopulationForVehicle, matrixWay);


                //for (int i = 0; i < sizePopulation; i++)//по наборам
                //{
                //    costWay = CostWayGeneticAlgorithm(matrixWay, newPopulation[i]);
                //    if (costWay < bestCostWay)
                //    {
                //        bestCostWay = costWay;

                //        tmp = GetSet(newPopulation, i);
                //        for (int j = 0; j < population[i].Length; j++)//по наборам
                //        {
                //            bestWay[j] = tmp[j];
                //        }

                //    }
                //}

                //for (int i = 0; i < sizePopulation; i++)//по наборам
                //{
                //    Mutation(newPopulation);
                //}


                numberIterationForVehicle--;
            }


        }

        public void DistributionOfCustomersByDepot(double[][] distanceFromOrdersToDepot)
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            //for (int i = 0; i < CustomerData.Count; i++)
            //{
            //    double min = 999999999;
            //    int minDepot = -1;
            //    for (int j = 0; j < DepotData.Count; j++)
            //    {
            //        if (min > distanceFromOrdersToDepot[j][i])
            //        {
            //            min = distanceFromOrdersToDepot[j][i];
            //            minDepot = j;
            //        }
            //    }

            //    CustomerData[i].depot = DepotData[minDepot].id;
            //}

            for (int i = 0; i < distanceFromOrdersToDepot[0].Length; i++)
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

                CustomerData.Where(x => x.orderAddress == i).ForEach(x => x.depot = DepotData[minDepot].id);
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