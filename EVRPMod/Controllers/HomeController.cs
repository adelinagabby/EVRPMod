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

        public struct VehicleOrders
        {
            public int Vehicle;
            public List<CustomerAndKit> customersAndKits;
        }

        public struct Vehicle
        {
            public int VehicleId;
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



        public static List<int[]> Sort(List<PopulationAndCost> populationAndCosts)
        {
            double[] costWays = new double[populationAndCosts.Count];

            for (int i = 0; i < costWays.Length; i++)
            {
                costWays[i] = populationAndCosts[i].cost;
            }


            double tmp;
            for (int i = 1, j; i < costWays.Length; ++i) // цикл проходов, i - номер прохода
            {
                tmp = costWays[i];
                for (j = i - 1; j >= 0 && costWays[j] < tmp; --j) // поиск места элемента в готовой последовательности
                {
                    costWays[j + 1] = costWays[j];    // сдвигаем элемент направо, пока не дошли 
                }
                costWays[j + 1] = tmp; // место найдено, вставить элемент
              
            }

            List<int[]> newPopulationAndCosts = new List<int[]>();

            for (int i = 0; i < costWays.Length; i++)
            {
                bool flag = false;
                int j = 0;
                while(flag == false)
                {
                    if(populationAndCosts[j].cost == costWays[i])
                    {
                        newPopulationAndCosts.Add(populationAndCosts[j].population);
                        populationAndCosts.RemoveAt(j);
                    }
                    j++;
                }
            }

            return newPopulationAndCosts;
        }


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
            EVRPModContext db = new EVRPModContext();


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


            List<VehiclesOfDepot> vehiclesOfDepots = new List<VehiclesOfDepot>();

            for (int k = 0; k < distanceMatrixBetweenCustomersAndDepots.Length; k++)
            {
                FindingShortestPaths(doubleDistanceMatrixBetweenCustomersAndDepots[k],db.depotData.Where(x=>x.orderInAlgoritm == k).First().id);
            }

            return Json(0);
        }

        

        public VehiclesOfDepot FindingShortestPaths(double[][] distanceMatrixBetweenCustomersAndDepots, int depotId)
        {

            EVRPModContext db = new EVRPModContext();
            var VehicleData = db.vehicleData.OrderBy(x => x.orderInAlgoritm).ToList();
            var KitType = db.kitType.ToList();
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
                    vehicles.Add(new Vehicle { VehicleId = objVehicle.id, carryingСapacity = (int)objVehicle.capacity, orderInAlgoritm = orderVehicleInAlgoritm, costRoads = (double)objVehicle.costRoads, serviceRoads = (double) objVehicle.serviceCost });
                    orderVehicleInAlgoritm++;
                }
            }

            //List<VehicleOrders> bestVehicleOrdering = new List<VehicleOrders>();
            СostOfJourney bestCostAndPopulation = new СostOfJourney() { cost = double.MaxValue};

            int numberOfAddress = distanceMatrixBetweenCustomersAndDepots.Length;
            int numberOfVehicles = vehicles.Count;

            int numberIterationForVehicle = 100;
            //int numberIterationForOrder = 100;

            //int[] bestWay = new int[matrixWay.Length + 1];
            //double bestCostWay = infinity;


            //List<int[]> populationForAddress = new List<int[]>();
            //List<int[]> newPopulationForAddress = new List<int[]>();


            List<int[]> populationForVehicle = new List<int[]>();
            List<int[]> newPopulationForVehicle = new List<int[]>();

            //List<int[]> populationForOrder = new List<int[]>();
            //List<int[]> newPopulationForOrder = new List<int[]>();

            int sizePopulationForVehicle = 10;
            //int sizePopulationForOrder = 10;

            //получаем перестановки для ТС
            populationForVehicle = GeneticAlgoritm.PrimaryPopulation(sizePopulationForVehicle, numberOfVehicles);


            //int[] tmpForVehicle;
            for (int i = 0; i < sizePopulationForVehicle; i++)//по наборам
            {
                //tmpForVehicle = GeneticAlgoritm.GetSet(populationForVehicle, i);
                newPopulationForVehicle.Add(GeneticAlgoritm.GetSet(populationForVehicle, i));
            }


            List<PopulationAndCost> populationsAndCosts = new List<PopulationAndCost>();
            

            //нахождение общего пути от депо и по клиентам
            int[] GeneralWayFromDepotToCustomers = new int[distanceMatrixBetweenCustomersAndDepots.Length];
            GeneralWayFromDepotToCustomers = GeneticAlgoritm.GeneticAlgorithm(distanceMatrixBetweenCustomersAndDepots, 10, 0);

            int[] CustomersOrder = new int[distanceMatrixBetweenCustomersAndDepots.Length];

            for (int i = 0; i < CustomersOrder.Length; i++)
            {
                CustomersOrder[i] = GeneralWayFromDepotToCustomers[i + 1];
            }



            //итерации по ТС
            while (numberIterationForVehicle > 0)
            {
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

                        //распределение заказов по ТС
                            for (int iCustomer = 0; iCustomer < CustomersOrder.Length; iCustomer++)
                            {
                                var CustomerOrder = db.customerData.Where(x => x.orderInAlgoritm == iCustomer).ToList();
                                for (int iOrder = 0; iOrder < CustomerOrder.Count; iOrder++)
                                {
                                    var tempVehicle = vehicles.Where(x => x.orderInAlgoritm == tmpForVehicle[iVehicle]).First();
                                    var tempKits = KitType.Where(x => x.id == CustomerOrder[iOrder].kitType).First();

                                    List<CustomerAndKit> tempCustomersAndKits = new List<CustomerAndKit>();
                                    if (tempVehicle.carryingСapacity - tempVehicle.loadOccupied > tempKits.weight * CustomerOrder[iOrder].count)
                                    {
                                        tempCustomersAndKits.Add(new CustomerAndKit() { Customer = CustomerOrder[iOrder].id, Kit = (int)CustomerOrder[iOrder].kitType, Count = (int)CustomerOrder[iOrder].count });
                                        tempVehicle.loadOccupied += (int)tempKits.weight * (int)CustomerOrder[iOrder].count;
                                    }
                                    else
                                    {
                                        int countKits = 0;
                                        if (tempVehicle.carryingСapacity - tempVehicle.loadOccupied > tempKits.weight)
                                        {
                                            tempVehicle.loadOccupied += (int)tempKits.weight;
                                            countKits++;
                                        }
                                        else
                                        {
                                            tempCustomersAndKits.Add(new CustomerAndKit()
                                            {
                                                Customer = CustomerOrder[iOrder].id,
                                                Kit = (int)CustomerOrder[iOrder].kitType,
                                                Count = countKits
                                            });
                                            countKits = 0;
                                            VehicleOrders tempVehicleOrders = new VehicleOrders() { Vehicle = vehicles[iVehicle].VehicleId, customersAndKits = tempCustomersAndKits };
                                        //добавить в список
                                        tempVehiclesOrdering.Add(tempVehicleOrders);
                                        iVehicle++;

                                        }
                                    }
                                }

                           
                            }

                        double costForPopulationOfVehicles = 0;
                        //узнать стоимость данной комбинации
                        for (iVehicle = 0; iVehicle < tempVehiclesOrdering.Count; iVehicle++)
                        {
                            
                            //сформировать матрицу расстояний для конктерного ТС

                            int[] addressOrderForAlgorithm = new int[tempVehiclesOrdering[iVehicle].customersAndKits.Count+1];
                            addressOrderForAlgorithm[0] = 0;
                            //цикл по клиентам
                            for (int iCustomer = 0; iCustomer < tempVehiclesOrdering[iVehicle].customersAndKits.Count; iCustomer++)
                            {
                                addressOrderForAlgorithm[iCustomer + 1] = (int) db.customerData.Where(x => x.id == tempVehiclesOrdering[iVehicle].customersAndKits[iCustomer].Customer).First().orderInAlgoritm;
                            }

                            //формируем матрицу расстояний для конкретного iVehicle ТС
                            double[,] vehicleSpecificDistanceMatrix = new double[addressOrderForAlgorithm.Length, addressOrderForAlgorithm.Length];
                            double[,] vehicleCostRoadMatrix = new double[addressOrderForAlgorithm.Length, addressOrderForAlgorithm.Length];

                            for (int i = 0; i < addressOrderForAlgorithm.Length; i++)
                            {
                                //vehicleSpecificDistanceMatrix[i] = new double[addressOrderForAlgorithm.Length];

                                for (int j = 0; j < addressOrderForAlgorithm.Length; j++)
                                {
                                    vehicleSpecificDistanceMatrix[i,j] = distanceMatrixBetweenCustomersAndDepots[addressOrderForAlgorithm[i]][addressOrderForAlgorithm[j]];
                                    vehicleCostRoadMatrix[i, j] = (int) db.costTable.Where(x => x.rowTable == addressOrderForAlgorithm[i] && x.columnTable == addressOrderForAlgorithm[j]).First().valueTable;
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


                                
                            BranchAndBoundaryMethod.bestCostWayBranchAndBoundaryMethod = double.MaxValue;
                            BranchAndBoundaryMethod.flag = false;
                            shortWayVehicle = BranchAndBoundaryMethod.Branch_And_Boundary_Method(vehicleSpecificDistanceMatrix, I, J, shortWayVehicle);



                        //double costForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleSpecificDistanceMatrix, shortWayVehicle);
                        // double costRoadForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleCostRoadMatrix, shortWayVehicle)*
                        // vehicles.Where(x=>x.VehicleId == tempVehiclesOrdering[iVehicle].Vehicle).First().costRoads;
                        double costForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleSpecificDistanceMatrix, shortWayVehicle)*
                              vehicles.Where(x => x.VehicleId == tempVehiclesOrdering[iVehicle].Vehicle).First().serviceRoads;
                        double costRoadForSpecificVehicle = BranchAndBoundaryMethod.CostWayBranchAndBoundaryMethod(vehicleCostRoadMatrix, shortWayVehicle) *
                            vehicles.Where(x => x.VehicleId == tempVehiclesOrdering[iVehicle].Vehicle).First().costRoads;
                        //умножть на стоимость ТС
                        costForPopulationOfVehicles += costForSpecificVehicle + costRoadForSpecificVehicle;
                        }

                      
                        if (costForPopulationOfVehicles<bestCostAndPopulation.cost)
                        {
                            bestCostAndPopulation.cost = costForPopulationOfVehicles;
                            bestCostAndPopulation.vehicleOrders = tempVehiclesOrdering;
                        }

                        //запишем перестановку и стоимость
                        populationsAndCosts.Add(new PopulationAndCost() { population = tmpForVehicle, cost = costForPopulationOfVehicles});

                        



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
                populationForVehicle = Sort(populationsAndCosts);
                //Кроссинговер
                populationForVehicle = GeneticAlgoritm.Crossing(populationForVehicle);
                //Мутация
                GeneticAlgoritm.Mutation(populationForVehicle);


                numberIterationForVehicle--;
            }

            VehiclesOfDepot VehiclesOfDepot = new VehiclesOfDepot() { depot = depotId, vehicleOrders = bestCostAndPopulation.vehicleOrders};

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