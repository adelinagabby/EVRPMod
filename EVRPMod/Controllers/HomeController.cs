using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
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
            public int Customer;
            public int Kit;
            public int Count;
        }


        public struct CustomerNameAndKitName
        {
            public string CustomerName;
            public int Kit;
            public string KitName;
            public int Count;
        }

        public struct VehicleOrders
        {
            public int Vehicle;
            public List<CustomerAndKit> customersAndKits;
        }


        public struct VehicleOrdersName
        {
            public string VehicleName;
            public List<CustomerNameAndKitName> customersAndKitsName;
        }
        public struct Vehicle
        {
            public int VehicleId;
            public string VehicleName;
            public int carryingСapacity;
            public int loadOccupied;
            public double serviceRoads;
            public double costRoads;
            public int orderInAlgoritm;
        }

        public struct VehiclesOfDepot
        {
            public int depot;
            public List<VehicleOrders> vehicleOrders;
        }
        public struct VehiclesOfDepotName
        {
            public string depotName;
            public List<VehicleOrdersName> vehicleOrdersName;
        }

        public struct СostOfJourney
        {
            public double cost;
            public List<VehicleOrders> vehicleOrders;
        }


        public struct PopulationAndCost
        {
            public double cost;
            public int[] population;
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
        public ActionResult CheckingForCorrectConnections()
        {
            string Result = "";

            EVRPModContext db = new EVRPModContext();
            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();
            var VehicleData = db.vehicleData.ToList();
            var VehicleInDepot = db.vehicleInDepot.ToList();
            var KitType = db.kitType.ToList();

            if(CustomerData.Count == 0)
            {
                Result = "Ошибка. Отсутствует информация о заказах";
                return Json(Result);
            }

            if (Result == "")
            {
                if (VehicleInDepot.Count == 0)
                {
                    Result = "Ошибка. Отсутствует информация о транспортных средствах в депо";
                    return Json(Result);
                }
            }
                if (Result == "")
            {
                for (int i = 0; i < CustomerData.Count; i++)
                {
                    if (KitType.FirstOrDefault(x => x.id == CustomerData[i].kitType) == null)
                    {
                        Result = "Ошибка. В заказе №" + CustomerData[i].id + " указан несуществующий вид комплекта";
                        return Json(Result);
                    }

                }
            }
            if (Result == "")
            {
                for (int i = 0; i < VehicleInDepot.Count; i++)
                {
                    if (DepotData.FirstOrDefault(x => x.id == VehicleInDepot[i].depotId) == null)
                    {
                        Result = "Ошибка. В \"Транспортные средства в депо\" указано несуществующее депо";
                        return Json(Result);
                    }

                    if (VehicleData.FirstOrDefault(x => x.id == VehicleInDepot[i].vehicleId) == null)
                    {
                        Result = "Ошибка.  В \"Транспортные средства в депо\" указано несуществующее транспортное средство";
                        return Json(Result);
                    }
                }
            }

            if (Result == "")
            {
                if(db.AlgorithmSettings.Where(x=>x.variable == "ConsideringTypeAndQualityOfRoads").FirstOrDefault().state == true && db.AlgorithmSettings.Where(x => x.variable == "RoadAccountingTablesAreSaved").FirstOrDefault().state == false)
                {
                    Result = "Ошибка. Были добавлены новые адреса. Информация о дорогах неактуальная";
                    return Json(Result);
                }
            }
            
                return Json(0);
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



        //public static List<int[]> Sort(List<PopulationAndCost> populationAndCosts)
        //{
        //    //double[] costWays = new double[populationAndCosts.Count];

        //    List<double> costWays = new List<double>();
        //    for (int i = 0; i < costWays.Count; i++)
        //    {
        //        costWays[i] = populationAndCosts[i].cost;
        //    }


        //    //double tmp;
        //    //for (int i = 1, j; i < costWays.Length; ++i) // цикл проходов, i - номер прохода
        //    //{
        //    //    tmp = costWays[i];
        //    //    for (j = i - 1; j >= 0 && costWays[j] < tmp; --j) // поиск места элемента в готовой последовательности
        //    //    {
        //    //        costWays[j + 1] = costWays[j];    // сдвигаем элемент направо, пока не дошли 
        //    //    }
        //    //    costWays[j + 1] = tmp; // место найдено, вставить элемент
              
        //    //}

        //    List<int[]> newPopulationAndCosts = new List<int[]>();

            

        //    for (int i = 0; i < costWays.Count; i++)
        //    {
        //        bool flag = false;
        //        int j = 0;
        //        while(flag == false)
        //        {
        //            if(populationAndCosts[j].cost == costWays[i])
        //            {
        //                newPopulationAndCosts.Add(populationAndCosts[j].population);
        //                populationAndCosts.RemoveAt(j);
        //                flag = true;
        //            }
        //            j++;
        //        }
        //    }

        //    return newPopulationAndCosts;
        //}


        [HttpPost]
        public ActionResult FindingMatrixsDistancesBetweenCustomersAndDepots(string[][] distanceMatrixBetweenCustomersAndDepots)
        {

            double[][] doubleDistanceMatrixBetweenCustomersAndDepots = new double[distanceMatrixBetweenCustomersAndDepots.Length][];

            for (int i = 0; i < distanceMatrixBetweenCustomersAndDepots.Length; i++)
            {
                doubleDistanceMatrixBetweenCustomersAndDepots[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
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

            //если учет дорог, то применяем метод Кини-Райфа
            if (db.AlgorithmSettings.Where(x => x.variable == "ConsideringTypeAndQualityOfRoads").FirstOrDefault().state == true)
            {
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
                    MatrixAverageSpeed, MatrixRoadQuality, MatrixAverageRoadIntensity);


            }


            DistributionOfCustomersByDepot(doubleDistanceMatrixBetweenCustomersAndDepots);

        


            List<CoordinateAndCount> MatrixCoordinateAllAddress = new List<CoordinateAndCount>();
            int orderAddress = -1;

            foreach (var itemDepot in DepotData)
            {
                //List<CoordinateAndCount> CoordinateAndCount = new List<CoordinateAndCount>();
                List<Coordinate> CoordinateAllAddress = new List<Coordinate>();
                CoordinateAllAddress.Add(new Coordinate() { latitude = itemDepot.latitude, longitude = itemDepot.longitude });

                int orderInAlgoritm = 0;
                itemDepot.orderInAlgoritm = orderInAlgoritm;
                orderInAlgoritm++;
                //CustomerData.Where(x => x.depot == itemDepot.id).First();
                CustomerData = db.customerData.Where(x => x.depot == itemDepot.id).OrderBy(x=>x.orderAddress).ToList();

                foreach (var itemCustomer in CustomerData)
                {
                    

                        //if (itemCustomer.orderAddress != orderAddress)
                        //{
                        //    orderInAlgoritm++;
                        //    CustomerData.Where(x => x.orderAddress == itemCustomer.orderAddress).ForEach(x => x.orderInAlgoritm = orderInAlgoritm);
                        //    // itemCustomer.orderInAlgoritm = orderInAlgoritm;
                        //    orderAddress = (int)itemCustomer.orderAddress;
                        //    CoordinateAllAddress.Add(new Coordinate() { latitude = itemCustomer.latitude, longitude = itemCustomer.longitude });
                        //}
                        if (itemCustomer.orderAddress != orderAddress)
                        {
                            
                            orderAddress = (int)itemCustomer.orderAddress;
                            CustomerData.Where(x => x.orderAddress == itemCustomer.orderAddress).ForEach(x => x.orderInAlgoritm = orderInAlgoritm);
                            // itemCustomer.orderInAlgoritm = orderInAlgoritm;
                            orderInAlgoritm++;
                            CoordinateAllAddress.Add(new Coordinate() { latitude = itemCustomer.latitude, longitude = itemCustomer.longitude });
                        }
                    
                }
                //CoordinateAndCount.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
                MatrixCoordinateAllAddress.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
            }




            db.SaveChanges();
            //?CoordinateAndCount
            return Json(MatrixCoordinateAllAddress);
        }

        [HttpPost]
        public ActionResult FindingShortestPaths(string[][][] distanceMatrixBetweenCustomersAndDepots)
        {
            EVRPModContext db = new EVRPModContext();
            var DepotData = db.depotData.OrderBy(x => x.orderAddress).ToList();
            var CustomerData = db.customerData.OrderBy(x => x.orderAddress).ToList();

            double[][][] doubleDistanceMatrixBetweenCustomersAndDepots = new double[distanceMatrixBetweenCustomersAndDepots.Length][][];

            for (int k = 0; k < distanceMatrixBetweenCustomersAndDepots.Length; k++)
            {
                doubleDistanceMatrixBetweenCustomersAndDepots[k] = new double[distanceMatrixBetweenCustomersAndDepots[k].Length][];
                for (int i = 0; i < distanceMatrixBetweenCustomersAndDepots[k].Length; i++)
                {
                    doubleDistanceMatrixBetweenCustomersAndDepots[k][i] = new double[distanceMatrixBetweenCustomersAndDepots[k].Length];
                    for (int j = 0; j < distanceMatrixBetweenCustomersAndDepots[k].Length; j++)
                    {

                        //Делим на 1000 для того, чтобы перевести метры в км
                        doubleDistanceMatrixBetweenCustomersAndDepots[k][i][j] = Convert.ToDouble(distanceMatrixBetweenCustomersAndDepots[k][i][j].Replace(".", ","))/1000;
                    }
                }
            }



            List<VehiclesOfDepot> vehiclesOfDepots = new List<VehiclesOfDepot>();

            for (int k = 0; k < distanceMatrixBetweenCustomersAndDepots.Length; k++)
            {

                //если учет дорог, то применяем метод Кини-Райфа
                if (db.AlgorithmSettings.Where(x => x.variable == "ConsideringTypeAndQualityOfRoads").FirstOrDefault().state == true)
                {
                    var AverageSpeedData = db.AverageSpeedTable.ToList();
                    var AverageRoadIntensityData = db.AverageRoadIntensityTable.ToList();
                    var RoadQualityData = db.RoadQualityTable.ToList();

                    //Получение матриц дорог между депо и клиентами
                    double[][] MatrixAverageSpeed = new double[distanceMatrixBetweenCustomersAndDepots[0].Length][];
                    double[][] MatrixRoadQuality = new double[distanceMatrixBetweenCustomersAndDepots[0].Length][];
                    double[][] MatrixAverageRoadIntensity = new double[distanceMatrixBetweenCustomersAndDepots[0].Length][];


                    MatrixAverageSpeed[0] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
                    MatrixRoadQuality[0] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
                    MatrixAverageRoadIntensity[0] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];

                    MatrixAverageSpeed[0][0] = 0;
                    MatrixRoadQuality[0][0] = 0;
                    MatrixAverageRoadIntensity[0][0] = 0;

                    for (int j = 1; j < distanceMatrixBetweenCustomersAndDepots[k].Length; j++)
                    {
                        
                        MatrixAverageSpeed[0][j] = AverageSpeedData.Where(x => x.rowTable == DepotData[k].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                        MatrixRoadQuality[0][j] = RoadQualityData.Where(x => x.rowTable == DepotData[k].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                        MatrixAverageRoadIntensity[0][j] = AverageRoadIntensityData.Where(x => x.rowTable == DepotData[k].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                    }

                    for (int i = 1; i < distanceMatrixBetweenCustomersAndDepots[k].Length; i++)
                    {
                        MatrixAverageSpeed[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
                        MatrixRoadQuality[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];
                        MatrixAverageRoadIntensity[i] = new double[distanceMatrixBetweenCustomersAndDepots[0].Length];

                        MatrixAverageSpeed[i][0] = AverageSpeedData.Where(x => x.rowTable == CustomerData[i].orderAddress && x.columnTable == DepotData[k].orderAddress).First().valueTable ?? 0;
                        MatrixRoadQuality[i][0] = RoadQualityData.Where(x => x.rowTable == CustomerData[i].orderAddress&& x.columnTable == DepotData[k].orderAddress).First().valueTable ?? 0;
                        MatrixAverageRoadIntensity[i][0] = AverageRoadIntensityData.Where(x => x.rowTable == CustomerData[i].orderAddress && x.columnTable == DepotData[k].orderAddress).First().valueTable ?? 0;


                        for (int j = 1; j < distanceMatrixBetweenCustomersAndDepots[k].Length; j++)
                        {
                            MatrixAverageSpeed[i][j] = AverageSpeedData.Where(x => x.rowTable == CustomerData[i].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                            MatrixRoadQuality[i][j] = RoadQualityData.Where(x => x.rowTable == CustomerData[i].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                            MatrixAverageRoadIntensity[i][j] = AverageRoadIntensityData.Where(x => x.rowTable == CustomerData[i].orderAddress && x.columnTable == CustomerData[j].orderAddress).First().valueTable ?? 0;
                        }
                    }


                    doubleDistanceMatrixBetweenCustomersAndDepots[k] = MethodKiniRayfa.ModificationOfMatrix(doubleDistanceMatrixBetweenCustomersAndDepots[k],
                    MatrixAverageSpeed, MatrixRoadQuality, MatrixAverageRoadIntensity);


                }
                //FindingShortestPaths(doubleDistanceMatrixBetweenCustomersAndDepots[k],db.depotData.Where(x=>x.orderInAlgoritm == k).First().id);
                vehiclesOfDepots.Add(FindingShortestPaths(doubleDistanceMatrixBetweenCustomersAndDepots[k], DepotData[k].id));

            }

            
            return Json(GettingTextResult(vehiclesOfDepots));
        }

        public List<VehiclesOfDepotName> GettingTextResult(List<VehiclesOfDepot> vehiclesOfDepots)
        {
            //string result = "";

            EVRPModContext db = new EVRPModContext();
            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();
            var VehicleData = db.vehicleData.ToList();
            var KitType = db.kitType.ToList();

            List<VehiclesOfDepotName> vehiclesOfDepotNames = new List<VehiclesOfDepotName>();


            for (int i = 0; i < vehiclesOfDepots.Count; i++)
            {
                VehiclesOfDepotName vehiclesOfDepotName = new VehiclesOfDepotName();
                vehiclesOfDepotName.depotName = DepotData.Where(x => x.id == vehiclesOfDepots[i].depot).First().name;
                vehiclesOfDepotName.vehicleOrdersName = new List<VehicleOrdersName>();
                for (int j = 0; j < vehiclesOfDepots[i].vehicleOrders.Count; j++)
                {
                    VehicleOrdersName vehicleOrdersName = new VehicleOrdersName();
                    vehicleOrdersName.VehicleName = VehicleData.Where(x => x.id == vehiclesOfDepots[i].vehicleOrders[j].Vehicle).First().name;
                    vehicleOrdersName.customersAndKitsName = new List<CustomerNameAndKitName>();
                    for (int k = 0; k < vehiclesOfDepots[i].vehicleOrders[j].customersAndKits.Count; k++)
                    {
                        CustomerNameAndKitName customerNameAndKitName = new CustomerNameAndKitName();
                        customerNameAndKitName.Count = vehiclesOfDepots[i].vehicleOrders[j].customersAndKits[k].Count;
                        customerNameAndKitName.CustomerName = CustomerData.Where(x => x.id == vehiclesOfDepots[i].vehicleOrders[j].customersAndKits[k].Customer).First().address;
                        customerNameAndKitName.Kit = vehiclesOfDepots[i].vehicleOrders[j].customersAndKits[k].Kit;
                        customerNameAndKitName.KitName = KitType.Where(x => x.id == vehiclesOfDepots[i].vehicleOrders[j].customersAndKits[k].Kit).First().name;
                        vehicleOrdersName.customersAndKitsName.Add(customerNameAndKitName);
                    }
                    vehiclesOfDepotName.vehicleOrdersName.Add(vehicleOrdersName);
                }
                vehiclesOfDepotNames.Add(vehiclesOfDepotName);
            }


            return vehiclesOfDepotNames;
        }

        public VehiclesOfDepot FindingShortestPaths(double[][] distanceMatrixBetweenCustomersAndDepots, int depotId)
        {

            EVRPModContext db = new EVRPModContext();
            var VehicleData = db.vehicleData.OrderBy(x => x.orderInAlgoritm).ToList();
            var KitType = db.kitType.ToList();
            var VehicleInDepot = db.vehicleInDepot.Where(x => x.depotId == depotId).ToList();

            List<Vehicle> vehicles = new List<Vehicle>();
            //int Vehicle;
            //int carryingСapacity;
            //int loadOccupied;
            int orderVehicleInAlgoritm = 0;
            for (int i = 0; i < VehicleInDepot.Count; i++)
            {
                var objVehicle = VehicleData.Where(x => x.id == VehicleInDepot[i].vehicleId).First();

                for (int j = 0; j < VehicleInDepot[i].count; j++)
                {
                    // Vehicle tempVehicle = new Vehicle { VehicleId = objVehicle.id, carryingСapacity = (int)objVehicle.capacity, orderInAlgoritm  = orderInAlgoritm};
                    vehicles.Add(new Vehicle { VehicleId = objVehicle.id, carryingСapacity = (int)objVehicle.capacity, orderInAlgoritm = orderVehicleInAlgoritm, costRoads = (double)objVehicle.costRoads, serviceRoads = (double)objVehicle.serviceCost });
                    orderVehicleInAlgoritm++;
                }
            }

            //List<VehicleOrders> bestVehicleOrdering = new List<VehicleOrders>();
            СostOfJourney bestCostAndPopulation = new СostOfJourney() { cost = double.MaxValue };

            int numberOfAddress = distanceMatrixBetweenCustomersAndDepots.Length;
            int numberOfVehicles = vehicles.Count;

            int numberIterationForVehicle = 10;
            //int numberIterationForOrder = 100;

            //int[] bestWay = new int[matrixWay.Length + 1];
            //double bestCostWay = infinity;


            //List<int[]> populationForAddress = new List<int[]>();
            //List<int[]> newPopulationForAddress = new List<int[]>();


            List<int[]> populationForVehicle = new List<int[]>();
            //List<int[]> newPopulationForVehicle = new List<int[]>();

            //List<int[]> populationForOrder = new List<int[]>();
            //List<int[]> newPopulationForOrder = new List<int[]>();

            int sizePopulationForVehicle = 10;
            //int sizePopulationForOrder = 10;

            //получаем перестановки для ТС
            populationForVehicle = GeneticAlgoritm.PrimaryPopulation(sizePopulationForVehicle, numberOfVehicles);


            //int[] tmpForVehicle;
            //for (int i = 0; i < sizePopulationForVehicle; i++)//по наборам
            //{
            //    //tmpForVehicle = GeneticAlgoritm.GetSet(populationForVehicle, i);
            //    newPopulationForVehicle.Add(GeneticAlgoritm.GetSet(populationForVehicle, i));
            //}


            List<PopulationAndCost> populationsAndCosts = new List<PopulationAndCost>();


            //нахождение общего пути от депо и по клиентам
            int[] GeneralWayFromDepotToCustomers = new int[distanceMatrixBetweenCustomersAndDepots.Length];
            GeneralWayFromDepotToCustomers = GeneticAlgoritm.GeneticAlgorithm(distanceMatrixBetweenCustomersAndDepots, 10, 0);
            //GeneralWayFromDepotToCustomers = BranchAndBoundaryMethod.Branch_And_Boundary_Method()
            int[] CustomersOrder = new int[distanceMatrixBetweenCustomersAndDepots.Length - 1];

            for (int i = 0; i < CustomersOrder.Length; i++)
            {
                CustomersOrder[i] = GeneralWayFromDepotToCustomers[i + 1];
            }



            //итерации по ТС
            while (numberIterationForVehicle > 0)
            {
                populationsAndCosts = new List<PopulationAndCost>();
                int[] tmpForVehicle;
                //для каждой популяции ТС
                for (int iPopulationForVehicle = 0; iPopulationForVehicle < sizePopulationForVehicle; iPopulationForVehicle++)
                {
                    //получаем набор ТС
                    tmpForVehicle = GeneticAlgoritm.GetSet(populationForVehicle, iPopulationForVehicle);


                    //populationForOrder = GeneticAlgoritm.PrimaryPopulation(sizePopulationForOrder, numberOfVehicles);

                    //int[] tmpForOrder;
                    //for (int i = 0; i < sizePopulationForOrder; i++)//по наборам
                    // {
                    //tmpForOrder = GeneticAlgoritm.GetSet(populationForOrder, i);
                    //    newPopulationForOrder.Add(GeneticAlgoritm.GetSet(populationForOrder, i));
                    // }
                    ////для каждой популяции заказов
                    //for (int iPopulationForOrder = 0; iPopulationForVehicle < sizePopulationForOrder; iPopulationForOrder++)
                    //{
                    List<VehicleOrders> tempVehiclesOrdering = new List<VehicleOrders>();

                    //bool flag = false;

                    //int[] tmpForOrder;
                    // tmpForOrder = GeneticAlgoritm.GetSet(populationForOrder, iPopulationForOrder);

                    //while (flag == false)
                    //{
                    //for (int iVehicle = 0; iVehicle < tmpForVehicle.Length; iVehicle++)
                    //{
                    int iVehicle = 0;
                    //int iCustomer = 0;
                    var tempVehicle = vehicles.Where(x => x.orderInAlgoritm == tmpForVehicle[iVehicle]).First();
                    List<CustomerAndKit> tempCustomersAndKits = new List<CustomerAndKit>();
                   // List<CustomerAndKit> tempRemainsCustomersAndKits = new List<CustomerAndKit>();
                    //распределение заказов по ТС
                    for (int iCustomer = 0; iCustomer < CustomersOrder.Length; iCustomer++)
                    {
                        var CustomerOrder = db.customerData.Where(x => x.orderInAlgoritm == CustomersOrder[iCustomer] && x.depot == depotId).ToList();
                        for (int iOrder = 0; iOrder < CustomerOrder.Count; iOrder++)
                        {
                            //var tempVehicle = vehicles.Where(x => x.orderInAlgoritm == tmpForVehicle[iVehicle]).First();
                            var tempKits = KitType.Where(x => x.id == CustomerOrder[iOrder].kitType).First();

                            int remains = CustomerOrder[iOrder].count ?? 0;
                            while (remains > 0)
                            {
                                //Если все комплекты помещаются целиком
                                if (tempVehicle.carryingСapacity - tempVehicle.loadOccupied > tempKits.weight * remains)
                                {
                                    tempCustomersAndKits.Add(new CustomerAndKit() { Customer = CustomerOrder[iOrder].id, Kit = (int)CustomerOrder[iOrder].kitType, Count = remains });
                                    tempVehicle.loadOccupied += (int)tempKits.weight * remains;
                                    remains = 0;

                                }
                                else
                                {
                                    int countKits = 0;
                                  
                                    while (true)
                                    {
                                        //Заполняем ТС по одному комплекту
                                        if (tempVehicle.carryingСapacity - tempVehicle.loadOccupied > tempKits.weight)
                                        {
                                            tempVehicle.loadOccupied += (int)tempKits.weight;
                                            countKits++;
                                        }
                                        else
                                        {

                                            if (countKits != 0)
                                            {
                                                tempCustomersAndKits.Add(new CustomerAndKit()
                                                {
                                                    Customer = CustomerOrder[iOrder].id,
                                                    Kit = (int)CustomerOrder[iOrder].kitType,
                                                    Count = countKits
                                                });
                                            }
                                            else
                                            {
                                                //Если в ТС уже нет места ни для одного комплекта заказа

                                                //iVehicle++;
                                            }

                                            //var temp = new List<CustomerAndKit>();



                                            VehicleOrders tempVehicleOrders = new VehicleOrders() { Vehicle = vehicles[tmpForVehicle[iVehicle]].VehicleId, customersAndKits = tempCustomersAndKits };
                                            //добавить в список
                                            tempVehiclesOrdering.Add(tempVehicleOrders);
                                            iVehicle++;
                                            tempVehicle = vehicles.Where(x => x.orderInAlgoritm == tmpForVehicle[iVehicle]).FirstOrDefault();
                                            tempCustomersAndKits = new List<CustomerAndKit>();
                                            remains -= countKits;
                                            break;
                                        }
                                    }
                                }
                                
                            }


                        }


                    }

                    if (tempCustomersAndKits.Count != 0)
                    {
                        VehicleOrders tempVehicleOrders = new VehicleOrders() { Vehicle = vehicles[iVehicle].VehicleId, customersAndKits = tempCustomersAndKits };
                        //добавить в список
                        tempVehiclesOrdering.Add(tempVehicleOrders);
                    }
                    double costForPopulationOfVehicles = 0;
                    //узнать стоимость данной комбинации
                    for (iVehicle = 0; iVehicle < tempVehiclesOrdering.Count; iVehicle++)
                    {

                        //сформировать матрицу расстояний для конктерного ТС
                        
                            int[] addressOrderForAlgorithm = new int[tempVehiclesOrdering[iVehicle].customersAndKits.Count + 1];
                            addressOrderForAlgorithm[0] = 0;
                            //цикл по клиентам
                            for (int iCustomer = 0; iCustomer < tempVehiclesOrdering[iVehicle].customersAndKits.Count; iCustomer++)
                            {
                                addressOrderForAlgorithm[iCustomer + 1] = (int)db.customerData.Where(x => x.id == tempVehiclesOrdering[iVehicle].customersAndKits[iCustomer].Customer).First().orderInAlgoritm;
                            }

                            //формируем матрицу расстояний для конкретного iVehicle ТС
                            //double[,] vehicleSpecificDistanceMatrix = new double[addressOrderForAlgorithm.Length, addressOrderForAlgorithm.Length];
                            double[][] vehicleSpecificDistanceMatrix = new double[addressOrderForAlgorithm.Length][];
                            double[][] vehicleCostRoadMatrix = new double[addressOrderForAlgorithm.Length][];

                            for (int i = 0; i < addressOrderForAlgorithm.Length; i++)
                            {
                                vehicleSpecificDistanceMatrix[i] = new double[addressOrderForAlgorithm.Length];
                                vehicleCostRoadMatrix[i] = new double[addressOrderForAlgorithm.Length];
                                for (int j = 0; j < addressOrderForAlgorithm.Length; j++)
                                {
                                    vehicleSpecificDistanceMatrix[i][j] = distanceMatrixBetweenCustomersAndDepots[addressOrderForAlgorithm[i]][addressOrderForAlgorithm[j]];
                                    vehicleCostRoadMatrix[i][j] = (int)db.costTable.Where(x => x.rowTable == addressOrderForAlgorithm[i] && x.columnTable == addressOrderForAlgorithm[j]).First().valueTable;
                                }
                            }

                            int[] I = new int[addressOrderForAlgorithm.Length];
                            int[] J = new int[addressOrderForAlgorithm.Length];
                            int[] shortWayVehicle = new int[addressOrderForAlgorithm.Length];

                        for (int i = 0; i < addressOrderForAlgorithm.Length; i++)
                        {
                            I[i] = int.MaxValue;
                            J[i] = int.MaxValue;
                        }


                        


                        if (tempVehiclesOrdering[iVehicle].customersAndKits.Count > 1)
                        {

                            double[][] vehicleSpecificDistanceMatrixBranchAndBoundaryMethod = new double[vehicleSpecificDistanceMatrix.Length][];

                            for (int i = 0; i < vehicleSpecificDistanceMatrixBranchAndBoundaryMethod.Length; i++)
                            {
                                vehicleSpecificDistanceMatrixBranchAndBoundaryMethod[i] = new double[vehicleSpecificDistanceMatrixBranchAndBoundaryMethod.Length];
                                for (int j = 0; j < vehicleSpecificDistanceMatrixBranchAndBoundaryMethod.Length; j++)
                                {
                                    if (vehicleSpecificDistanceMatrix[i][j] == 0)
                                        vehicleSpecificDistanceMatrixBranchAndBoundaryMethod[i][j] = double.MaxValue;
                                    else
                                        vehicleSpecificDistanceMatrixBranchAndBoundaryMethod[i][j] = vehicleSpecificDistanceMatrix[i][j];
                                }
                            }   

                            BranchAndBoundaryMethod.bestCostWayBranchAndBoundaryMethod = double.MaxValue;
                            BranchAndBoundaryMethod.flag = false;
                            I[0] = 0;

                            shortWayVehicle = BranchAndBoundaryMethod.Branch_And_Boundary_Method(vehicleSpecificDistanceMatrixBranchAndBoundaryMethod, I, J, shortWayVehicle);
                           
                            //shortWayVehicle = GeneticAlgoritm.GeneticAlgorithm(vehicleSpecificDistanceMatrix, 10);
                        }
                        else
                        {

                            shortWayVehicle[0] = 0;
                            shortWayVehicle[0] = 1;
                        }
                        //double costForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleSpecificDistanceMatrix, shortWayVehicle);
                        // double costRoadForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleCostRoadMatrix, shortWayVehicle)*
                        // vehicles.Where(x=>x.VehicleId == tempVehiclesOrdering[iVehicle].Vehicle).First().costRoads;
                        double costForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleSpecificDistanceMatrix, shortWayVehicle) *
                              vehicles.Where(x => x.VehicleId == tempVehiclesOrdering[iVehicle].Vehicle).First().serviceRoads;
                        double costRoadForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleCostRoadMatrix, shortWayVehicle) *
                            vehicles.Where(x => x.VehicleId == tempVehiclesOrdering[iVehicle].Vehicle).First().costRoads;
                        //умножть на стоимость ТС
                        costForPopulationOfVehicles += costForSpecificVehicle + costRoadForSpecificVehicle;
                    }


                    if (costForPopulationOfVehicles < bestCostAndPopulation.cost)
                    {
                        bestCostAndPopulation.cost = costForPopulationOfVehicles;
                        bestCostAndPopulation.vehicleOrders = tempVehiclesOrdering;
                    }

                    //запишем перестановку и стоимость
                    populationsAndCosts.Add(new PopulationAndCost() { population = tmpForVehicle, cost = costForPopulationOfVehicles });





                    //while (iVehicle < tmpForVehicle.Length)
                    //{
                    //    int iCustomer = 0;
                    //    VehicleOrders tempVehicleOrders = new VehicleOrders() { };
                    //    //for (int iCustomer = 0; iCustomer < tmpForOrder.Length; iCustomer++)
                    //    //{
                    //    while (iCustomer < tmpForOrder.Length)
                    //    {
                    //        //получаем все заказы клиента
                    //        //получаем ВСЕ КОМПЛЕКТЫ ОТ КЛИЕНТА
                    //        var CustomerOrder = db.customerData.Where(x => x.orderInAlgoritm == iCustomer).ToList();
                    //        for (int iOrder = 0; iOrder < CustomerOrder.Count; iOrder++)
                    //        {
                    //            var tempVehicle = vehicles.Where(x => x.orderInAlgoritm == tmpForVehicle[iVehicle]).First();
                    //            var tempKits = KitType.Where(x => x.id == CustomerOrder[iOrder].kitType).First();
                    //            if (tempVehicle.carryingСapacity - tempVehicle.loadOccupied > tempKits.weight * CustomerOrder[iOrder].count)
                    //            {

                    //            }
                    //        }
                    //    }
                    //}
                    //}
                    //}

                    //}







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

                }

                //Сортируем и создаем новую популяцию
                //populationForVehicle = Sort(populationsAndCosts);
                populationForVehicle = populationsAndCosts.OrderBy(x => x.cost).Select(x=>x.population).ToList();
                //Кроссинговер
                populationForVehicle = GeneticAlgoritm.Crossing(populationForVehicle);
                //Мутация
                GeneticAlgoritm.Mutation(populationForVehicle);


                numberIterationForVehicle--;
            }


            //Сдвигаем последние заказы
            for (int i = bestCostAndPopulation.vehicleOrders.Count-1; i > 0; i--)
            {
                if (bestCostAndPopulation.vehicleOrders[i].customersAndKits[0].Customer == bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits[bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits.Count - 1].Customer)
                {
                    double tempWeight = 0;

                    for (int j = 0; j < bestCostAndPopulation.vehicleOrders[i].customersAndKits.Count; j++)
                    {
                        tempWeight += bestCostAndPopulation.vehicleOrders[i].customersAndKits[j].Count * (double)KitType.Where(x => x.id == bestCostAndPopulation.vehicleOrders[i].customersAndKits[j].Kit).First().weight;
                    }

                    if (VehicleData.Where(x => x.id == bestCostAndPopulation.vehicleOrders[i].Vehicle).First().capacity >=
                    (tempWeight + (double)KitType.Where(x => x.id == bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits[bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits.Count - 1].Kit).First().weight * bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits[bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits.Count - 1].Count))
                    {
                        var tempObj = bestCostAndPopulation.vehicleOrders[i].customersAndKits[0];


                        bestCostAndPopulation.vehicleOrders[i].customersAndKits.RemoveAt(0);
                        bestCostAndPopulation.vehicleOrders[i].customersAndKits.Insert(0, new CustomerAndKit()
                        {
                            Customer = tempObj.Customer,
                            Kit = tempObj.Kit,
                            Count = tempObj.Count +
                            bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits[bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits.Count - 1].Count
                        });
                        bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits.RemoveAt(bestCostAndPopulation.vehicleOrders[i - 1].customersAndKits.Count - 1);

                    }
                }

            }

            VehiclesOfDepot VehiclesOfDepot = new VehiclesOfDepot() { depot = depotId, vehicleOrders = bestCostAndPopulation.vehicleOrders };

            return VehiclesOfDepot;
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